using CommonDX;
using SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using SharpDX;

namespace SharpGraphics {

	public class SharpTexture {

		public D3DDevice device;
		public Texture2D texture;
		public SamplerState sampler;
		public ShaderResourceView resource;

		public SharpTexture(D3DDevice device,Texture2D texture) {
			this.device = device;
			this.texture = texture;
			initSampler(TextureAddressMode.Wrap,TextureAddressMode.Wrap,Filter.MinMagMipLinear);
			resource = new ShaderResourceView(device,texture);
		}

		public SharpTexture(D3DDevice device,string filename) : this(device,TextureLoader.CreateTexture2DFromBitmap(device,TextureLoader.LoadBitmap(GFX.imagingFactory,filename))) {

		}

		public void initSampler(TextureAddressMode addressU,TextureAddressMode addressV,Filter filter) {
			// color sampler state
			var samplerStateDesc = new SamplerStateDescription() {
				Filter = filter,
				AddressU = addressU,
				AddressV = addressV,
				AddressW = TextureAddressMode.Wrap
			};
			sampler = new SamplerState(device,samplerStateDesc);
		}

		public void initSampler(TextureAddressMode address,Filter filter) {
			initSampler(address,address,filter);
		}

		public void initSampler(Color borderColor,Filter filter) {
			// color sampler state
			var samplerStateDesc = new SamplerStateDescription() {
				Filter = filter,
				AddressU = TextureAddressMode.Border,
				AddressV = TextureAddressMode.Border,
				BorderColor = borderColor
			};
			sampler = new SamplerState(device,samplerStateDesc);
		}

	}

}
