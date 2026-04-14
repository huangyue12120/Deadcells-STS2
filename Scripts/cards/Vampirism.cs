using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Deadcells.Scripts.cards;

//吸血
//所有敌人获得 !M! 层 流血 . NL 回复敌人 流血 层数总和的生命. NL 消耗 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class Vampirism() : DeadcellsCardModel(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override bool Red => true;
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BleedingPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BleedingPower>(4)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BleedingPower>(enemy, base.DynamicVars["BleedingPower"].BaseValue, base.Owner.Creature, this);
        }
        int num = await Task.Run(() => base.CombatState.HittableEnemies.Sum((Creature enemy) => enemy.GetPowerAmount<BleedingPower>()));
        if (num > 0)
        {
            await CreatureCmd.Heal(base.Owner.Creature, num, true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BleedingPower"].UpgradeValueBy(1);
    }

}