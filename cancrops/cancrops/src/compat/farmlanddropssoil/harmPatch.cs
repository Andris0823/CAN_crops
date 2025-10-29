using HarmonyLib;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using FarmlandDropsSoil;

namespace cancrops.src.compat.farmlanddropssoil
{
    [HarmonyPatch]
    public class harmPatch
    {


        public static void Postfix_FarmlandDropsSoilBehavior_GetDrops(FarmlandDropsSoilBehavior __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer,
            ref float dropChanceMultiplier, ref EnumHandling handling, ref ItemStack[] __result)
        {
           /* if ((byPlayer?.WorldData.CurrentGameMode == EnumGameMode.Creative) ||
                (world.BlockAccessor.GetBlockEntity(pos) is not CANBlockEntityFarmland farmland))
            {
                return;
            }
            handling = EnumHandling.PreventSubsequent;


            var nutrients = farmland.Nutrients.Zip(farmland.OriginalFertility,
                (current, original) => current / original).Min();

            // If this nutrient is below 95%, there is a chance the soil won't drop.
            if ((nutrients < 0.95) && (world.Rand.NextDouble() > nutrients))
                return;

            var fertility = __instance.block.FirstCodePart(2);
            var block = world.GetBlock(new AssetLocation("game", $"soil-{fertility}-none"));
            __result = __result.Append(new ItemStack(block)).ToArray();*/
        }
    }
       
}
