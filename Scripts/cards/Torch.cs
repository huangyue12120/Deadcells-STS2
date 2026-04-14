using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//火把
//造成 !D! 点伤害,给予 !M! 层 燃烧 . NL 如果目标已有 燃烧 额外给予 !Burns! 层
[Pool(typeof(DeadcellsCardPool))]
public sealed class Torch() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BurnsPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5, ValueProp.Move),
        new PowerVar<BurnsPower>(6),
        new IntVar("ExtraBurnsPower", 3)
    };

    protected override bool ShouldGlowGoldInternal => base.CombatState.HittableEnemies.Any(e => e.HasPower<BurnsPower>());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        if (cardPlay.Target.HasPower<BurnsPower>())
        {
            decimal BurnsTotal = base.DynamicVars["BurnsPower"].BaseValue + base.DynamicVars["ExtraBurnsPower"].BaseValue;
            await PowerCmd.Apply<BurnsPower>(cardPlay.Target, BurnsTotal, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<BurnsPower>(cardPlay.Target, base.DynamicVars["BurnsPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnsPower"].UpgradeValueBy(3);
    }

}
