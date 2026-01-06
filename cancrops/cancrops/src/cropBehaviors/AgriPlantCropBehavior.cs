using System;
using cancrops.src.BE;
using cancrops.src.genetics;
using cancrops.src.implementations;
using cancrops.src.utility;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace cancrops.src.cropBehaviors
{
    public class AgriPlantCropBehavior : CropBehavior
    {
        public AgriPlantCropBehavior(Block block) : base(block)
        {
            
        }
        public override void OnPlanted(ICoreAPI api, ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel)
        {
            base.OnPlanted(api, itemslot, byEntity, blockSel);
            if (api.Side == EnumAppSide.Server && !itemslot.Empty)
            {
                string text = itemslot.Itemstack.Collectible.Code.Domain + ":" + itemslot.Itemstack.Collectible.LastCodePart();
                if (text == "bellpepper")
                {
                    return;
                }
                AgriPlant agriPlant = cancrops.GetPlants().getPlant(text);
                if (agriPlant == null)
                {
                    return;
                }
                if (api.World.BlockAccessor.GetBlockEntity<CANBECrop>(blockSel.Position.UpCopy()) is CANBECrop beCrop)
                {
                    Genome seedGenome = CommonUtils.GetSeedGenomeFromAttribute(itemslot.Itemstack);
                    beCrop.Genome = seedGenome;
                    beCrop.agriPlant = agriPlant;
                    beCrop.MarkDirty();
                }
                else if (api.World.BlockAccessor.GetBlockEntity<CANBECrop>(blockSel.Position) is CANBECrop beCropSame)
                {
                    Genome seedGenome = CommonUtils.GetSeedGenomeFromAttribute(itemslot.Itemstack);
                    beCropSame.Genome = seedGenome;
                    beCropSame.agriPlant = agriPlant;
                    beCropSame.MarkDirty();
                }
            }
        }
        public override bool TryGrowCrop(ICoreAPI api, IFarmlandBlockEntity farmland, double currentTotalHours, int newGrowthStage, ref EnumHandling handling)
        {
            //(farmland as BlockEntityFarmland)
            if (api.World.BlockAccessor.GetBlockEntity<CANBECrop>(farmland.Pos.UpCopy()) is CANBECrop beCrop)
            {
                if(beCrop.weedStage != WeedStage.NONE)
                {
                    return false;
                }
                bool res = AgriPlantRequirmentChecker.CheckAgriPlantRequirements(beCrop);
                if(!res)
                {
                    handling = EnumHandling.PreventDefault;                   
                }
                return res;
            }
            return false;
        }
    }
}
