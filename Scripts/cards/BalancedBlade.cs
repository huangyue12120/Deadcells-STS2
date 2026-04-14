using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//均衡之刃
//造成9点伤害。在本场战斗中每打出一次，伤害增加3，此后每次伤害增加量+1。
[Pool(typeof(DeadcellsCardPool))]
public sealed class BalancedBlade() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool Red => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    private int _usenumber;

    private int UseNumber
    {
        get
        {
            return _usenumber;
        }
        set
        {
            AssertMutable();
            _usenumber = value;
        }
    }

    private int TotalDamage { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9, ValueProp.Move),
        new ExtraDamageVar(3),
        new IntVar("Increase", 1),

        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("TotalDamage").WithMultiplier((card, _)=>
        {
            if (card is BalancedBlade bb)
            {
                if (card.IsUpgraded)
                {
                    return 9 + (9 + bb._usenumber) * bb._usenumber / 2;
                }
                else
                {
                    return 9 + (5 + bb._usenumber) * bb._usenumber / 2;
                }
            }

            return 0;
        })
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars["TotalDamage"].PreviewValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await Task.Run(() => UseNumber += 1);
    }

    public override Task BeforeCombatStart()
    {
        UseNumber = 0;
        TotalDamage = 0;
        base.DynamicVars.Damage.ResetToBase();
        return base.BeforeCombatStart();
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        UseNumber = 0;
        TotalDamage = 0;
        base.DynamicVars.Damage.ResetToBase();
        return base.AfterCombatVictory(room);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }

}
