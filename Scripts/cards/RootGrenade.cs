using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//藤蔓手雷
//对所有敌人造成 !D! 点伤害 并给予 !M! 层 藤蔓缠绕 . NL 获得 !B! 点 格挡 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class RootGrenade() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Sly
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Grenade];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<GrenadePower>(),
        HoverTipFactory.FromPower<ConstrictPower>(),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5, ValueProp.Move),
        new BlockVar(10,ValueProp.Move),
        new PowerVar<ConstrictPower>(2)

    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ConstrictPower>(enemy, base.DynamicVars["ConstrictPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1);
        base.DynamicVars.Block.UpgradeValueBy(2);
        base.DynamicVars["ConstrictPower"].UpgradeValueBy(1);
    }

}
