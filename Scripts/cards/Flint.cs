using BaseLib.Utils;
using Deadcells.Scripts.character;
using Deadcells.Scripts.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

[Pool(typeof(DeadcellsCardPool))]
public sealed class Flint() : DeadcellAmplifyCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override bool Green => true;

    protected override int extraCost => 1;

    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<AmplifyKeyword>(),
        HoverTipFactory.FromPower<BurnsPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10, ValueProp.Move),
        new PowerVar<BurnsPower>(5),
        new IntVar("EnergyNeed", 2)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bite", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        if (IsAmplified)
        {
            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<BurnsPower>(enemy, DynamicVars["BurnsPower"].BaseValue, Owner.Creature, this);
                if(enemy == cardPlay.Target)
                {
                    await PowerCmd.Apply<BurnsPower>(enemy, DynamicVars["BurnsPower"].BaseValue, Owner.Creature, this);
                }
            }
        }
        else
        {
            await PowerCmd.Apply<BurnsPower>(cardPlay.Target, DynamicVars["BurnsPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
        base.DynamicVars["BurnsPower"].UpgradeValueBy(4);
    }
}
