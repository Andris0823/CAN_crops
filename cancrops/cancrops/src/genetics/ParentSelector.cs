using cancrops.src.BE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cancrops.src.genetics
{
    public class ParentSelector
    {
        public List<CANBECrop> selectAndOrder(IEnumerable<CANBECrop> neighbours, Random random)
        {
            /*foreach(var it in neighbours)
            {
                if(it.HasRipeCrop())
                {
                    var f = 3;
                }
            }
            var c = neighbours
                    // Mature crops only
                    .Where(x => x.HasRipeCrop()).ToList();
            c = c.Where(x => (!cancrops.config.onlyFertileCropsCanSpread())).ToList();
            c = c.OrderByDescending(sorter).ToList();
            c = c.Where(x => this.rollFertility(x, random)).ToList();*/
            return neighbours
                    // Mature crops only
                    .Where(x => x.GetCropStageWithout() >= x.agriPlant.AllowSourceStage)
                    //.Where(x => x.HasRipeCrop())
                    // Fertile crops only
                    //.Where(x => (!cancrops.config.onlyFertileCropsCanSpread))
                    // Sort based on fertility stat
                    .OrderByDescending(sorter)
                    // Roll for fertility stat
                    .Where(x => this.rollFertility(x, random))
                    // Collect successful passes
                    .ToList();
        }
        protected int sorter(CANBECrop crop)
        {
            return cancrops.config.maxFertility - crop?.Genome.Fertility.Dominant.Value ?? 1;
        }
        protected bool rollFertility(CANBECrop crop, Random random)
        {
            int tm = random.Next(cancrops.config.maxFertility);
            return true;
            return tm < (crop?.Genome.Fertility.Dominant.Value ?? 1);
        }
    }
}
