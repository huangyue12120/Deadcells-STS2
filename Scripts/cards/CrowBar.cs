using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;
//撬棍
//造成 !D! 点伤害，如果上一张牌是 *技能牌 ，则造成双倍伤害。
[Pool(typeof(DeadcellsCardPool))]
public sealed class CrowBar() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8, ValueProp.Move),
        new IntVar("DoubleDamage", 2)
    };

    private bool WasLastCardPlayedSkill
    {
        get
        {
            CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card != this);
            if (cardPlayStartedEntry == null)
            {
                return false;
            }
            return cardPlayStartedEntry.CardPlay.Card.Type == CardType.Skill;
        }
    }

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedSkill;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (WasLastCardPlayedSkill)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue * base.DynamicVars["DoubleDamage"].BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
    }
}
