using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//墓碑
//造成14Dmg,斩杀时给予所有敌人20/26层灾厄。\n杀死所有[gold]灾厄[/gold]大于等于当前生命值的敌人。
[Pool(typeof(DeadcellsCardPool))]
public sealed class Tombstone() : DeadcellsCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool Green => true;
    protected override HashSet<CardTag> CanonicalTags => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Retain
    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<DoomPower>()
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14, ValueProp.Move),
        new PowerVar<DoomPower>(20)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bite", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
        {
            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<DoomPower>(enemy, base.DynamicVars.Doom.BaseValue, base.Owner.Creature, this);
            }
            await DoomPower.DoomKill(DoomPower.GetDoomedCreatures(base.CombatState.HittableEnemies));
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DoomPower"].UpgradeValueBy(6);
    }

}
