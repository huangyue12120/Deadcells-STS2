using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;


//火焰手雷
//给予所有敌人 !M! 层 燃烧 并造成 !D! 点伤害 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class FireGrenade() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
         CardKeyword.Sly
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [DeadcellsCardTag.Grenade];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
         HoverTipFactory.FromPower<GrenadePower>(),
         HoverTipFactory.FromPower<BurnsPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BurnsPower>(6),
        new DamageVar(2, ValueProp.Move)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy == null) continue;
            await PowerCmd.Apply<BurnsPower>(enemy, base.DynamicVars["BurnsPower"].BaseValue, base.Owner.Creature, this);
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash", null, null)
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnsPower"].UpgradeValueBy(3);
    }
}
