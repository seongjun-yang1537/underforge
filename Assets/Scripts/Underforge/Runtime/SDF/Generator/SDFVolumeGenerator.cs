using Unity.Jobs;

namespace Underforge
{
    public static class SDFVolumeGenerator
    {
        public static JobHandle Generate(SDFVolume volume, SDFGenerateConfig config, JobHandle dependency = default)
        {
            SDFGenerateJob job = new SDFGenerateJob
            {

            };
        }
    }
}