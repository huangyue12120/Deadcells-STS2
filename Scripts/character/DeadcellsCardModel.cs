using BaseLib.Abstracts;
using Deadcells.Scripts.powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Deadcells.Scripts.character;

public abstract class DeadcellsCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true,
    bool autoAdd = true)
    : CustomCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
{
    //卡图
    public override string PortraitPath => $"res://Deadcells/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    //卡背

    public enum DeadcellsCardTag
    {
        None,
        Grenade, //炸弹
        Shield, //盾牌
        SingleBlood
    }

    protected virtual bool Red => false;
    protected virtual bool Green => false;
    protected virtual bool Purple => false;
    protected virtual bool Gold => false;
    protected virtual bool Gray => false;

    private string GetColorString()
    {
        List<string> trueColors = new List<string>();
        if (Red) trueColors.Add("red");
        if (Green) trueColors.Add("green");
        if (Purple) trueColors.Add("purple");
        if (Gold) trueColors.Add("gold");
        if (Gray) trueColors.Add("gray");

        if (trueColors.Count == 1)
        {
            return trueColors[0];
        }

        if (trueColors.Count == 2 && !Gold && !Gray)
        {
            if (Red && Green && !Purple) return "red2_green2";
            if (Red && Purple && !Green) return "red2_purple2";
            if (Green && Purple && !Red) return "purple2_green2";
        }

        return "gray";
    }

    protected virtual int GrenadeCooldownTime => 1;

    private string getCardFramePath()
    {
        string prefixText = "res://Deadcells/images/card_bg/bg";
        string postfixText = "dc_p.png";
        string typeText = this.Type switch
        {
            CardType.Attack => "attack",
            CardType.Skill => "skill",
            CardType.Power => "power",
            CardType.Status => "skill",
            _ => "skill"
        };

        string colorText = GetColorString();

        return $"{prefixText}_{colorText}_{typeText}_{postfixText}";
    }

    public override Texture2D? CustomFrame => ResourceLoader.Load<Texture2D>(getCardFramePath(), null, ResourceLoader.CacheMode.Reuse);


    private HashSet<DeadcellsCardTag>? _dcCardTag;

    public virtual IEnumerable<DeadcellsCardTag> DCCDTags => _dcCardTag ?? (_dcCardTag = DeadcellsCardTags);

    protected virtual HashSet<DeadcellsCardTag> DeadcellsCardTags => new HashSet<DeadcellsCardTag>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        if (this.DCCDTags.Contains(DeadcellsCardTag.Grenade))
        {
            if (!this.Keywords.Contains(CardKeyword.Exhaust))
            {
                GrenadePower grenadePower = base.Owner.Creature.GetPower<GrenadePower>();
                if (grenadePower == null)
                {
                    await PowerCmd.Apply<GrenadePower>(base.Owner.Creature, 1, base.Owner.Creature, this);
                    grenadePower = base.Owner.Creature.GetPower<GrenadePower>();
                }
                await grenadePower.AddGrenadeToCooldown(this, this.GrenadeCooldownTime);
                await CardCmd.Exhaust(choiceContext, this);
            }
        }
    }
}
