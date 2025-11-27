using Unity.Jobs;

namespace Underforge
{
    public static class SDFVolumeGenerator
    {
        public static JobHandle Generate(SDFVolume volume, SDFGenerateConfig config, JobHandle dependency = default)
        {
            SDFGenerateJob job = new SDFGenerateJob
            {
                volume = volume,
                type = config.type,
                offset = config.offset,
                noiseScale = config.noiseScale,
                noiseAmp = config.noiseAmplitude,
                noiseOffset = config.noiseSeed
            };

            return job.Schedule(volume.densities.Length, 64, dependency);
        }
    }
}