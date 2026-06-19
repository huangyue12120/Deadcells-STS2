using BaseLib.Utils;
using Deadcells.Scripts.character;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Deadcells.Scripts.cards;

//炼金卡宾枪
//随机给予敌人 1 层中毒3次。抽2张牌。
[Pool(typeof(DeadcellsCardPool))]
public sealed class AlchemyCarbine() : DeadcellsCardModel(1, CardType.Skill, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [];

    protected override bool Purple => true;

    private readonly Color _vfxTint = new Color("83eb85");

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<PoisonPower>(1),
        new RepeatVar(3),
        new CardsVar(2)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {

    };

    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PoisonPower>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        Vector2 lastPos = Vector2.Zero;
        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
        {
            Creature enemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
            if (enemy == null)
            {
                continue;
            }
            if (TestMode.IsOff)
            {
                if (i == 0)
                {
                    lastPos = NCombatRoom.Instance.GetCreatureNode(base.Owner.Creature).VfxSpawnPosition;
                }
                NCreature targetNode = NCombatRoom.Instance.GetCreatureNode(enemy);
                if (targetNode != null)
                {
                    NItemThrowVfx child = NItemThrowVfx.Create(lastPos, targetNode.GetBottomOfHitbox(), ModelDb.Potion<PoisonPotion>().Image);
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
                    lastPos = targetNode.VfxSpawnPosition;
                    await Cmd.Wait(0.5f);
                    NSplashVfx child2 = NSplashVfx.Create(targetNode.VfxSpawnPosition, _vfxTint);
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child2);
                    NLiquidOverlayVfx child3 = NLiquidOverlayVfx.Create(enemy, _vfxTint);
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child3);
                    NGaseousImpactVfx child4 = NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, _vfxTint);
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child4);
                }
            }
            await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, base.DynamicVars.Poison.BaseValue, base.Owner.Creature, this);
        }

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(3);
    }

}
