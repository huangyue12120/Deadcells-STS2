namespace Deadcells.Scripts.cards;
//思考加速
//回合开始根据战斗情况 NL 将 !M! 张 合适的牌 置入你的手中.
//TODO: 这张牌怎么实现再议！
//[Pool(typeof(DeadcellsCardPool))]
//public sealed class ReflectionSpeedUp() : DeadcellsCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
//{
//    protected override bool Gold => true;
//    protected override HashSet<CardTag> CanonicalTags => [];

//    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
//    {

//    };

//    protected override HashSet<DeadcellsCardTag> DeadcellsCardTags => [];

//    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
//    {

//    };

//    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
//    {
//        new DamageVar(8, ValueProp.Move)
//    };

//    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//    {

//    }

//    protected override void OnUpgrade()
//    {

//    }

//}
