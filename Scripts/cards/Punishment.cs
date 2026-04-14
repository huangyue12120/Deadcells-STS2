using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//惩罚之盾
//获得 !B! 点 格挡 . NL 随机打出 !M! 张 *攻击牌 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Punishment() : DeadcellsCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Shield];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(10, ValueProp.Move),
        new CardsVar(1)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        IEnumerable<CardModel> forCombat = CardFactory.GetForCombat(base.Owner, from c in ModelDb.CardPool<DeadcellsCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                                                                                where c.Type == CardType.Attack
                                                                                select c, base.DynamicVars.Cards.IntValue, base.Owner.RunState.Rng.CombatCardGeneration);
        foreach (CardModel cardModel in forCombat)
        {
            cardModel.AddKeyword(CardKeyword.Exhaust);
            cardModel.AddKeyword(CardKeyword.Ethereal);
            await CardCmd.AutoPlay(choiceContext, cardModel, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }

}
