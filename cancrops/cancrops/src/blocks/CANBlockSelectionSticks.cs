using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cancrops.src.BE;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace cancrops.src.blocks
{
    public class CANBlockSelectionSticks : Block, ITexPositionSource
    {
        private ITexPositionSource tmpTextureSource;
        private string curType;
        public Dictionary<string, AssetLocation> tmpAssets = new Dictionary<string, AssetLocation>();
        public Size2i AtlasSize
        {
            get
            {
                return this.tmpTextureSource.AtlasSize;
            }
        }
        private TextureAtlasPosition getOrCreateTexPos(AssetLocation texturePath)
        {
            TextureAtlasPosition texPos = (this.api as ICoreClientAPI).BlockTextureAtlas[texturePath];
            if (texPos == null && !(this.api as ICoreClientAPI).BlockTextureAtlas.GetOrInsertTexture(texturePath, out var _, out texPos))
            {
                (this.api as ICoreClientAPI).World.Logger.Warning(string.Concat("For render in block ", this.Code, ", item {0} defined texture {1}, no such texture found."), "", texturePath);
                return (this.api as ICoreClientAPI).BlockTextureAtlas.UnknownTexturePosition;
            }

            return texPos;
        }
        public TextureAtlasPosition this[string textureCode]
        {
            get
            {
                if (tmpAssets.TryGetValue(textureCode, out var assetCode))
                {
                    return this.getOrCreateTexPos(assetCode);
                }
                TextureAtlasPosition pos = this.tmpTextureSource[this.curType + "-" + textureCode];
                if (pos == null)
                {
                    pos = this.tmpTextureSource[textureCode];
                }
                if (pos == null)
                {
                    pos = (this.api as ICoreClientAPI).BlockTextureAtlas.UnknownTexturePosition;
                }
                return pos;
            }
        }
        public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
        {
            string cacheKey = "selectionsticksMeshRefs" + base.FirstCodePart(0);
            Dictionary<string, MultiTextureMeshRef> meshrefs = ObjectCacheUtil.GetOrCreate<Dictionary<string, MultiTextureMeshRef>>(capi, cacheKey, () => new Dictionary<string, MultiTextureMeshRef>());
            string type = itemstack.Attributes.GetString("type", "oak");

            this.tmpAssets["generic"] = new AssetLocation("game:block/wood/debarked/" + type + ".png");
            string key = string.Concat(new string[]
            {
                type
            });
            if (!meshrefs.TryGetValue(key, out renderinfo.ModelRef))
            {
                CompositeShape cshape = this.Shape.Clone();
                Vec3f rot = (this.ShapeInventory == null) ? null : new Vec3f(this.ShapeInventory.rotateX, this.ShapeInventory.rotateY, this.ShapeInventory.rotateZ);

                MeshData mesh = this.GenMesh(capi, type, cshape, rot);
                meshrefs[key] = (renderinfo.ModelRef = capi.Render.UploadMultiTextureMesh(mesh));
            }
        }
        public MeshData GenMesh(ICoreClientAPI capi, string type, CompositeShape cshape, Vec3f rotation = null)
        {
            Shape shape = this.GetShape(capi, type, cshape).Clone();
            ITesselatorAPI tesselator = capi.Tesselator;
            this.tmpAssets["generic"] = new AssetLocation("game:block/wood/debarked/" + type + ".png");
            if (shape == null)
            {
                return new MeshData(true);
            }
            this.curType = type;
            MeshData mesh;
            tesselator.TesselateShape("selectionsticks", shape, out mesh, this, (rotation == null) ? new Vec3f(this.Shape.rotateX, this.Shape.rotateY, this.Shape.rotateZ) : rotation, 0, 0, 0, null, null);
            return mesh;
        }
        public Shape GetShape(ICoreClientAPI capi, string type, CompositeShape cshape)
        {
            if (((cshape != null) ? cshape.Base : null) == null)
            {
                return null;
            }
            ITesselatorAPI tesselator = capi.Tesselator;
            this.tmpTextureSource = tesselator.GetTextureSource(this, 0, true);
            AssetLocation shapeloc = cshape.Base.WithPathAppendixOnce(".json").WithPathPrefixOnce("shapes/");
            Shape result = Vintagestory.API.Common.Shape.TryGet(capi, shapeloc).Clone();
            this.curType = type;
            return result;
        }
        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
        {
            ItemStack stack = new ItemStack(this, 1);
            CANBECrossSticks be = world.BlockAccessor.GetBlockEntity(pos) as CANBECrossSticks;
            if (be != null)
            {
                stack.Attributes.SetString("type", be.type);
            }
            else
            {
                stack.Attributes.SetString("type", "oak");
            }
            return stack;
        }
        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            return base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
        }
        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
        {
            base.OnBlockPlaced(world, blockPos, byItemStack);
        }
        public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
        {
            if(base.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                if(world.BlockAccessor.GetBlockEntity(blockSel.Position.DownCopy()) is BlockEntityFarmland be)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
