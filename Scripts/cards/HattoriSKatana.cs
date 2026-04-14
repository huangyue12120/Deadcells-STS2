using BaseLib.Utils;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Deadcells.Scripts.cards;

//服部武士刀
//造成 !D! 点伤害 !M! 次. NL 每打出一次伤害段数加一最多五段. NL 如果该牌至少打出三段则获得 !CN! 点 敏捷 和 力量 . NL 如果该牌费用不为0则 *使用后会返回手牌 .
[Pool(typeof(DeadcellsCardPool))]
public sealed class HattoriSKatana() : DeadcellsCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
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
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(1),
        new PowerVar<StrengthPower>(1)
    };

    protected override bool ShouldGlowGoldInternal => base.DynamicVars.Repeat.IntValue >= 3;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        if (base.DynamicVars.Repeat.IntValue >= 3)
        {
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        }

        if (this.EnergyCost.GetAmountToSpend() != 0)
        {
            await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
        }
        await UpdateRepeatNumber();
    }

    private Task UpdateRepeatNumber()
    {
        if (base.DynamicVars.Repeat.IntValue < 5)
        {
            base.DynamicVars.Repeat.UpgradeValueBy(1);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        base.DynamicVars.Repeat.ResetToBase();
        return base.AfterCombatVictory(room);
    }
}
