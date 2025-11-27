namespace Underforge
{
    public enum SDFGenerateType
    {
        Empty,  // 텅 빔 (모든 값이 양수)
        Full,   // 꽉 참 (모든 값이 음수)
        Plane,  // 평평한 바닥
        Perlin  // 울퉁불퉁한 지형
    }
}