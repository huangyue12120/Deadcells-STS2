using BaseLib.Abstracts;
using BaseLib.Utils;
using Deadcells.Scripts.cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Deadcells.Scripts.relics;

//刺猬背包
//拾取后选择将卡组中的一张技能牌替换为翻滚，每打出一张翻滚自动打出抽牌堆中的一张攻击牌
[Pool(typeof(DeadcellRelicPool))]
public sealed class BackpackSk : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    // 小图标
    public override string PackedIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Deadcells/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Roll>()
    };

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        List<CardModel> cards = (from c in PileType.Deck.GetPile(base.Owner).Cards
                                 orderby c.Rarity, c.Id
                                 where c.IsBasicStrikeOrDefend && c.Tags.Contains(CardTag.Strike)
                                 select c).ToList();
        if (cards.Count != 0)
        {
            foreach (CardModel item in await CardSelectCmd.FromSimpleGrid(new BlockingPlayerChoiceContext(), cards, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, base.DynamicVars.Cards.IntValue)))
            {
                await CardPileCmd.RemoveFromDeck(item);
                CardModel card = base.Owner.RunState.CreateCard<Roll>(base.Owner);
                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
            }
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is Roll)
        {
            List<CardModel> items = PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c.Type == CardType.Skill && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
            CardModel cardModel = base.Owner.RunState.Rng.Shuffle.NextItem(items);
            if (cardModel != null)
            {
                await CardCmd.AutoPlay(context, cardModel, null);
            }
        }
    }
}
