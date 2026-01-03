using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cancrops.src.BE;
using cancrops.src.blocks;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace cancrops.src.items
{
    public class CANItemAntiWeed: Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            handling = EnumHandHandling.PreventDefaultAction;
        }
        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (blockSel == null)
            {
                return false;
            }
            Block farmland = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);
            if (farmland is CANBlockSelectionSticks)
            {
                BlockEntity be = byEntity.World.BlockAccessor.GetBlockEntity(blockSel.Position);

                if (be is CANBECrossSticks beSticks)
                {
                    beSticks.OnCultivating(slot, byEntity);
                    if (beSticks.weedStage == WeedStage.NONE && beSticks.weedProtectionTicks < 50)
                    {
                        if(cancrops.config.weedProtectionTicks.TryGetValue(slot?.Itemstack?.Collectible.Code.Path, out var ticks))
                        {
                            beSticks.weedProtectionTicks = ticks;
                            beSticks.MarkDirty(true);
                            if (byEntity is EntityPlayer player)
                            {
                                slot.Itemstack.Collectible.DamageItem(this.api.World, player, slot, 1);
                            }
                        }
                        
                    }
                }
                return true;
            }
            return false;
        }
    }
}
