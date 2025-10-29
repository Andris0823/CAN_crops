using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cancrops.src.genetics;
using cancrops.src.implementations;
using cancrops.src.utility;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace cancrops.src.BE
{
    public class CANBECrop: BlockEntity
    {
        public Genome Genome;
        public AgriPlant agriPlant;
        private WeedStage weedStage = WeedStage.NONE;
        private ICoreClientAPI capi;
        public static Random rand = new Random();
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            if (api.Side == EnumAppSide.Client)
            {
                this.capi = (api as ICoreClientAPI);
            }
        }
        public int GetCropStageWithout()
        {
            Block crop = this.GetCrop();
            if (crop != null)
            {
                return this.GetCropStage(this.GetCrop());
            }
            return 0;
        }
        internal Block GetCrop()
        {
            Block block = this.Api.World.BlockAccessor.GetBlock(this.Pos);
            if (block == null || block.CropProps == null)
            {
                return null;
            }
            return block;
        }
        internal int GetCropStage(Block block)
        {
            int stage;
            int.TryParse(block.LastCodePart(0), out stage);
            return stage;
        }
        public void TryPropagateWeed(WeedStage weedStage, CANBECrossSticks crossSticks)
        {
            int resistance = 0;
            if (this.Genome != null)
            {
                resistance = this.Genome.Resistance.Dominant.Value;
            }
            cancrops.config.weedSpreadChancePerStage.TryGetValue((int)weedStage, out double weedChance);
            if (cancrops.config.weedResistanceByStat * resistance < weedChance + rand.Next(0, 10) / 100)
            {
                this.weedStage += 1;
                this.MarkDirty();
            }
        }
        private bool SetPlantStage(int stage)
        {
            Block block = this.GetCrop();
            if (block == null)
            {
                return false;
            }
            int currentGrowthStage = this.GetCropStage(block);

            Block nextBlock = this.Api.World.GetBlock(block.CodeWithParts(stage.ToString() ?? ""));
            if (nextBlock == null)
            {
                return false;
            }

            if (this.Api.World.BlockAccessor.GetBlockEntity(this.Pos) == null)
            {
                this.Api.World.BlockAccessor.SetBlock(nextBlock.BlockId, this.Pos);
            }
            else
            {
                this.Api.World.BlockAccessor.ExchangeBlock(nextBlock.BlockId, this.Pos);
            }
            return true;
        }
        public bool TryClipPlant(IPlayer byPlayer)
        {
            ItemSlot clipSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
            if(this.agriPlant == null)
            {
                return false;
            }
            if (agriPlant.Clip_products == null || this.GetCropStageWithout() < agriPlant.MinClipStage || agriPlant.MinClipStage == 0)
            {
                return false;
            }
            if (!SetPlantStage(agriPlant.ClipRollbackStage))
            {
                return false;
            }

            ItemStack clipTool = clipSlot.Itemstack;
            foreach (var drop in agriPlant.Clip_products.getRandom(rand))
            {
                if (drop.Item is ItemPlantableSeed)
                {
                    ITreeAttribute genomeTree = new TreeAttribute();
                    var fertilityStat = Genome.Fertility.Dominant.Value;
                    var mutativityStat = Genome.Mutativity.Dominant.Value;
                    foreach (Gene gene in Genome)
                    {
                        ITreeAttribute geneTree = new TreeAttribute();
                        geneTree.SetInt("D", Math.Min(1 - rand.Next(mutativityStat) + rand.Next(fertilityStat) + gene.Dominant.Value, gene.Dominant.Value));
                        geneTree.SetInt("R", Math.Min(1 - rand.Next(mutativityStat) + rand.Next(fertilityStat) + gene.Recessive.Value, gene.Recessive.Value));
                        genomeTree[gene.StatName] = geneTree;
                    }
                    drop.Attributes[cancrops.config.genome_tag] = genomeTree;
                }
                this.Api.World.SpawnItemEntity(drop, new Vec3d(this.Pos.X + 0.5, this.Pos.Y + 0.5, this.Pos.Z + 0.5));
            }
            clipTool.Collectible.DamageItem(this.Api.World, byPlayer.Entity, clipSlot, 1);
            return true;
        }

        ///////////////////////////////////////////////////////ATTRIBUTES//////////////////////////////////////////////////////////////////////////////////// 
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            if (tree.HasAttribute("genome"))
            {
                this.Genome = Genome.FromTreeAttribute(tree.GetTreeAttribute("genome"));
            }

            if (tree.HasAttribute("plant"))
            {
                this.agriPlant = cancrops.GetPlants()?.getPlant(tree.GetString("plant")) ?? null;
            }
            bool regenerateMesh = false;
            WeedStage newWeedStage = (WeedStage)tree.GetInt("weedStage", 0);
            if (this.weedStage != newWeedStage)
            {
                regenerateMesh = true;
                this.weedStage = newWeedStage;
            }

            if (worldForResolving.Side == EnumAppSide.Client && this.capi != null)
            {
                if (regenerateMesh)
                {
                   // GenRightMesh();
                }
            }
        }
        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            if (Genome != null)
            {
                tree["genome"] = this.Genome.AsTreeAttribute();
            }
            if (agriPlant != null)
            {
                tree.SetString("plant", this.agriPlant.Domain + ":" + this.agriPlant.Id);
            }
            tree.SetInt("weedStage", (int)this.weedStage);
        }
    }
}
