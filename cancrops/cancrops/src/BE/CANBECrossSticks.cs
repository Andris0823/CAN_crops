using System;
using System.Collections.Generic;
using cancrops.src.blocks;
using cancrops.src.genetics;
using cancrops.src.implementations;
using cancrops.src.utility;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace cancrops.src.BE
{
    public class CANBECrossSticks: BlockEntity
    {
        protected static Random rand = new Random();
        MeshData currentRightMesh;
        private WeedStage weedStage = WeedStage.NONE;
        private ICoreClientAPI capi;
        public string type = "oak";
        private MeshData ownMesh;
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            this.RegisterGameTickListener(new Action<float>(this.Update), 20000 + rand.Next(4000), 0);
            if(api.Side == EnumAppSide.Client)
            {
                if (this.currentRightMesh == null)
                {
                    this.currentRightMesh = this.GenRightMesh();
                    this.MarkDirty(true);
                }
                this.capi = (api as ICoreClientAPI);
                this.loadOrCreateMesh();
            }
        }
        internal MeshData GenRightMesh()
        {
            MeshData fullMesh = null;
            if (weedStage != WeedStage.NONE)
            {
                Shape weedShape = null;

                if (weedStage == WeedStage.LOW)
                {
                    weedShape = Api.Assets.TryGet("cancrops:shapes/weed-1.json").ToObject<Shape>();
                }
                else if (weedStage == WeedStage.MEDIUM)
                {
                    weedShape = Api.Assets.TryGet("cancrops:shapes/weed-2.json").ToObject<Shape>();
                }
                else if (weedStage == WeedStage.HIGH)
                {
                    weedShape = Api.Assets.TryGet("cancrops:shapes/weed-3.json").ToObject<Shape>();
                }

                if (weedShape != null)
                {
                    (Api as ICoreClientAPI).Tesselator.TesselateShape(this.Block, weedShape, out MeshData weedMesh);
                    fullMesh = weedMesh;
                }

            }
            /*Shape shape = null;
            shape = Api.Assets.TryGet("cancrops:shapes/selection_sticks_2.json").ToObject<Shape>();
            if (shape != null)
            {
                (Api as ICoreClientAPI).Tesselator.TesselateShape(this.Block, shape, out MeshData stickMesh);
                if (fullMesh != null)
                {
                    fullMesh.AddMeshData(stickMesh);
                }
                else
                {
                    fullMesh = stickMesh;
                }
            }*/

            if (fullMesh != null)
            {
                return fullMesh.Translate(new Vintagestory.API.MathTools.Vec3f(0, 0.9f, 0));
            }
            return fullMesh;
        }
        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            if (base.OnTesselation(mesher, tessThreadTesselator))
            {
                return true;
            }
            if (this.ownMesh == null)
            {
                return true;
            }

            mesher.AddMeshData(this.ownMesh, 1);
            return true;
            currentRightMesh = GenRightMesh();
            if (this.currentRightMesh != null)
            {
                mesher.AddMeshData(this.currentRightMesh);
            }
            return false;
        }
        /*public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            if (base.OnTesselation(mesher, tesselator))
            {
                return true;
            }
            if (this.ownMesh == null)
            {
                return true;
            }

            mesher.AddMeshData(this.ownMesh, 1);
            return true;
        }*/
        public void Update(float dt)
        {
            List<CANBECrop> neighbours = new();
            BlockPos tmpPos;
            foreach (var dir in BlockFacing.HORIZONTALS)
            {
                tmpPos = this.Pos.AddCopy(dir);
                BlockEntity blockEntityFarmland = this.Api.World.BlockAccessor.GetBlockEntity(tmpPos);
                if (cancrops.sapi.World.BlockAccessor.GetBlockEntity(this.Pos.AddCopy(dir)) is CANBECrop cbc)
                {
                    neighbours.Add(cbc);
                }
            }
            if (cancrops.GetAgriMutationHandler().handleCrossBreedTick(this, neighbours, rand))
            {
                //MinecraftForge.EVENT_BUS.post(new AgriCropEvent.Grow.Cross.Post(this));
            }
        }
        public bool spawnGenome(Genome genome, AgriPlant agriPlant)
        {
            this.setGenomeImpl(genome, agriPlant);
            return true;
        }
        protected void setGenomeImpl(Genome genome, AgriPlant agriPlant)
        {

            Block block = this.Api.World.GetBlock(new AssetLocation(agriPlant.Domain + ":crop-" + agriPlant.Id + "-1"));

            if (block == null)
            {
                return;
            }
            var seeds = CommonUtils.GetSeedItemStackFromFarmland(genome, agriPlant);
            DummySlot itemSlot = new DummySlot(seeds);
            this.Api.World.BlockAccessor.SetBlock(block.BlockId, this.Pos);
            CropBehavior[] behaviors = block.CropProps.Behaviors;
            var sel = new BlockSelection(this.Pos, BlockFacing.DOWN, block);
            for (int i = 0; i < behaviors.Length; i++)
            {
                behaviors[i].OnPlanted(this.Api, itemSlot, null, sel);
            }
            this.MarkDirty(true);
        }
        public void TryPropagateWeed(WeedStage weedStage)
        {
            if (this.weedStage == WeedStage.HIGH)
            {
                return;
            }
            BlockPos tmpPos;
            foreach (var dir in BlockFacing.HORIZONTALS)
            {
                tmpPos = this.Pos.AddCopy(dir);
                BlockEntity blockEntityFarmland = this.Api.World.BlockAccessor.GetBlockEntity(tmpPos);
                if (cancrops.sapi.World.BlockAccessor.GetBlockEntity(this.Pos.AddCopy(dir)) is CANBECrop cbc)
                {
                    cbc.TryPropagateWeed(this.weedStage, this);
                }
            }
        }
        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            if (((byItemStack != null) ? byItemStack.Attributes : null) != null)
            {
                string nowType = byItemStack.Attributes.GetString("type", "oak");
                if (nowType != this.type)
                {
                    this.type = nowType;
                    this.MarkDirty(false, null);
                }
            }
            base.OnBlockPlaced(byItemStack);
        }
        private void loadOrCreateMesh()
        {
            CANBlockSelectionSticks block = base.Block as CANBlockSelectionSticks;
            if (base.Block == null)
            {
                block = (this.Api.World.BlockAccessor.GetBlock(this.Pos) as CANBlockSelectionSticks);
                base.Block = block;
            }
            if (block == null)
            {
                return;
            }
            string cacheKey = "crateMeshes" + block.FirstCodePart(0);
            Dictionary<string, MeshData> meshes = ObjectCacheUtil.GetOrCreate<Dictionary<string, MeshData>>(this.Api, cacheKey, () => new Dictionary<string, MeshData>());
            CompositeShape cshape = block.Shape;
            if (((cshape != null) ? cshape.Base : null) == null)
            {
                return;
            }
            string meshKey = string.Concat(new string[]
            {
                this.type
            });
            MeshData mesh;
            if (!meshes.TryGetValue(meshKey, out mesh))
            {
                mesh = block.GenMesh(this.Api as ICoreClientAPI, this.type, cshape, new Vec3f(cshape.rotateX, cshape.rotateY, cshape.rotateZ));
                meshes[meshKey] = mesh;
            }
            this.ownMesh = mesh.Clone();
        }
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            bool regenerateMesh = false;
            WeedStage newWeedStage = (WeedStage)tree.GetInt("weedStage", 0);
            if (this.weedStage != newWeedStage)
            {
                regenerateMesh = true;
                this.weedStage = newWeedStage;
            }
            this.type = tree.GetString("type", "oak");

            if (this.Api != null && this.Api.Side == EnumAppSide.Client)
            {
                this.loadOrCreateMesh();
                this.MarkDirty(true, null);
            }
            if (worldForResolving.Side == EnumAppSide.Client && this.capi != null)
            {
                if (regenerateMesh)
                {
                    GenRightMesh();
                }
            }
        }
        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetInt("weedStage", (int)this.weedStage);
            if (this.type == null)
            {
                this.type = "oak";
            }
            tree.SetString("type", this.type);
        }
    }
}
