using BaseLib.Abstracts;
using Deadcells.Scripts.character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using static Deadcells.Scripts.character.DeadcellsCardModel;

namespace Deadcells.Scripts.powers;

//及时反应
//在你的回合开始前，如果当前有敌人的意图是攻击，则从抽牌堆中随机打出1张盾。
public sealed class InTimePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string? CustomBigBetaIconPath => $"res://Deadcells/images/powers/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {

    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {

    };

    private bool IntendsAttack = false;

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (Creature enemy in base.CombatState.Enemies)
        {
            if (enemy == null) continue;
            if (enemy.Monster.IntendsToAttack)
            {
                this.IntendsAttack = true;
            }
        }
        if (this.IntendsAttack)
        {
            this.Flash();
            for (int i = 0; i < this.Amount; i++)
            {
                List<CardModel> items = PileType.Draw.GetPile(base.Owner.Player).Cards.Where((CardModel c) => c is DeadcellsCardModel dc && dc.DCCDTags.Contains(DeadcellsCardTag.Shield) && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
                CardModel cardModel = base.Owner.Player.RunState.Rng.Shuffle.NextItem(items);
                if (cardModel != null)
                {
                    await CardCmd.AutoPlay(choiceContext, cardModel, null);
                }
            }
        }
    }
}