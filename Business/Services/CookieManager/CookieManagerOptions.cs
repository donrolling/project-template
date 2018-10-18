namespace Business.Services.Cookies {
	public class CookieManagerOptions {
		public bool AllowEncryption { get; set; } = true;
		public int? ChunkSize { get; set; } = 4050;
		public int DefaultExpireTimeInDays { get; set; } = 1;
		public bool ThrowForPartialCookies { get; set; } = true;
	}
}