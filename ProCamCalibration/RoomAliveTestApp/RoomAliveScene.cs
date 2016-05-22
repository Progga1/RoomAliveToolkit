using RoomAliveToolkit;
using SharpDX;
using SharpGraphics;
using GFX = SharpGraphics.GFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct3D11;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpMatrix = SharpDX.Matrix;

namespace RoomAliveTestApp {

	class RoomAliveScene {

		public const float PI = 3.1415926535f;

		public class Head {
			public SharpMatrix projMat;
			public Vector3 position;
			public float yaw = 0, pitch = -0.15f;

			public Head() {
				position = new Vector3(-0.6f,-0.5f,1.5f);
				projMat = SharpMatrix.PerspectiveFovLH(1.45f,1.35f,0.05f,10);
			}

			public SharpMatrix getView() {
				SharpMatrix result = getWorldTransform();
				result.Invert();
				return result;
			}

			public SharpMatrix getViewTransp() {
				SharpMatrix result = getView();
				result.Transpose();
				return result;
			}

			public SharpMatrix getProjection() {
				return projMat;
			}

			public SharpMatrix getProjectionTransp() {
				SharpMatrix result = projMat;
				result.Transpose();
				return result;
			}

			public SharpMatrix getWorldTransform() {
				SharpMatrix worldTransform = SharpMatrix.RotationX(pitch) * SharpMatrix.RotationY(yaw) * SharpMatrix.Translation(position);
				return worldTransform;
			}

			public SharpMatrix getWorldTransformTransp() {
				SharpMatrix result = getWorldTransform();
				result.Transpose();
				return result;
			}

		}

		public class HeadViewRendering {

			public static int textureWidth = 2048;
			public static int textureHeight = 1600;

			public D3DDevice device;
			public D3DDeviceContext context;
			public RenderSurface surface;
			public SharpTexture texture;
			public Viewport viewport;

			D3D11.RenderTargetView renderTargetView;
			D3D11.DepthStencilView depthStencilView;

			public RoomAliveScene scene;

			public HeadViewRendering(RoomAliveScene scene,RenderSurface surface) {
				this.scene = scene;
				this.surface = surface;
				this.device = surface.device;
				this.context = device.ImmediateContext;

				var userViewTextureDesc = new Texture2DDescription() {
					Width = textureWidth,
					Height = textureHeight,
					MipLevels = 1,
					ArraySize = 1,
					Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
					SampleDescription = new SharpDX.DXGI.SampleDescription(1,0),
					Usage = ResourceUsage.Default,
					BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
					CpuAccessFlags = CpuAccessFlags.None,
				};
				var userViewRenderTarget = new Texture2D(device,userViewTextureDesc);
				renderTargetView = new RenderTargetView(device,userViewRenderTarget);

				texture = new SharpTexture(device,userViewRenderTarget);
				texture.initSampler(TextureAddressMode.Clamp,Filter.MinMagMipLinear);

				depthStencilView = GFX.createZBuffer(device,textureWidth,textureHeight);

				viewport = new Viewport(0,0,textureWidth,textureHeight);
			}

			public float getRatioX() {
				return textureWidth/(float)textureHeight;
			}

			public void beginRendering() {
				context.ClearRenderTargetView(renderTargetView,new Color4(0,0,0,0));
				context.ClearDepthStencilView(depthStencilView,DepthStencilClearFlags.Depth,1,0);
				context.OutputMerger.SetRenderTargets(depthStencilView,renderTargetView);
				context.Rasterizer.SetViewport(viewport);
			}

			public void endRendering() {
				surface.restoreRenderTargets();
			}

		}

		public ProjectorCameraEnsemble ensemble;
		public Mesh roomMesh;
		public Head head;

		public RoomAliveScene(string calibrationFile,string meshFile) {
			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile(calibrationFile);
			roomMesh = new Mesh();
			roomMesh.flipTexY = true;
			roomMesh.LoadFromOBJFile(meshFile);

			head = new Head();
		}

	}

}
