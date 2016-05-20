using RoomAliveToolkit;
using SharpDX;
using SharpGraphics;
using GFX = SharpGraphics.GFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpMatrix = SharpDX.Matrix;

namespace RoomAliveTestApp {

	class RoomAliveScene {

		public class Head {
			public SharpMatrix projection;
			public Vector3 position;

			public Head() {
				position = new Vector3(-1,-0.5f,-0.2f);

				projection = SharpMatrix.PerspectiveFovLH(1.2f,1.2f,0.05f,10);
				//projection = GraphicsTransforms.PerspectiveFov(1.2f,1.2f,0.05f,10);
				//projection.Transpose();
				//projection[0,0] = -projection[0,0]; 
				//Console.WriteLine(projection);
				//Console.WriteLine(SharpMatrix.PerspectiveFovLH(1.2f,1.2f,0.05f,10));
			}

			public SharpMatrix getView() {
				SharpMatrix result = getWorldTransform();
				result.Invert();
				return result;
			}

			public SharpMatrix getProjection() {
				return projection;
			}

			public SharpMatrix getProjectionTransp() {
				SharpMatrix result = projection;
				result.Transpose();
				return result;
			}

			public SharpMatrix getWorldTransform() {
				SharpMatrix worldTransform = SharpMatrix.Translation(position);
				return worldTransform;
			}

			public SharpMatrix getWorldTransformTransp() {
				SharpMatrix result = getWorldTransform();
				result.Transpose();
				return result;
			}

		}

		public class HeadViewRendering {

			public static int userTextureWidth = 2048;
			public static int userTextureHeight = 1600;

			public D3DDevice device;
			public D3DDeviceContext context;
			public RenderSurface surface;

			D3D11.RenderTargetView userViewRenderTargetView;
			D3D11.DepthStencilView userViewDepthStencilView;

			public RoomAliveScene scene;

			public HeadViewRendering(RoomAliveScene scene,RenderSurface surface) {
				this.scene = scene;
				this.surface = surface;
				this.device = surface.device;
				this.context = device.ImmediateContext;

				var userViewTextureDesc = new D3D11.Texture2DDescription() {
					Width = userTextureWidth,
					Height = userTextureHeight,
					MipLevels = 1,
					ArraySize = 1,
					Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
					SampleDescription = new SharpDX.DXGI.SampleDescription(1,0),
					Usage = D3D11.ResourceUsage.Default,
					BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
					CpuAccessFlags = D3D11.CpuAccessFlags.None,
				};
				var userViewRenderTarget = new D3D11.Texture2D(device,userViewTextureDesc);
				userViewRenderTargetView = new D3D11.RenderTargetView(device,userViewRenderTarget);

				userViewDepthStencilView = GFX.createZBuffer(device,userTextureWidth,userTextureHeight);
			}

			public void beginRendering() {
				context.ClearRenderTargetView(userViewRenderTargetView,Color4.Black);
				context.OutputMerger.SetRenderTargets(userViewDepthStencilView);
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
