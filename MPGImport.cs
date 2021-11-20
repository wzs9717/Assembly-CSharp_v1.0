using System;
using System.Runtime.InteropServices;

public class MPGImport
{
	public enum mpg123_parms
	{
		MPG123_VERBOSE,
		MPG123_FLAGS,
		MPG123_ADD_FLAGS,
		MPG123_FORCE_RATE,
		MPG123_DOWN_SAMPLE,
		MPG123_RVA,
		MPG123_DOWNSPEED,
		MPG123_UPSPEED,
		MPG123_START_FRAME,
		MPG123_DECODE_FRAMES,
		MPG123_ICY_INTERVAL,
		MPG123_OUTSCALE,
		MPG123_TIMEOUT,
		MPG123_REMOVE_FLAGS,
		MPG123_RESYNC_LIMIT,
		MPG123_INDEX_SIZE,
		MPG123_PREFRAMES
	}

	public enum mpg123_param_flags
	{
		MPG123_FORCE_MONO = 7,
		MPG123_MONO_LEFT = 1,
		MPG123_MONO_RIGHT = 2,
		MPG123_MONO_MIX = 4,
		MPG123_FORCE_STEREO = 8,
		MPG123_FORCE_8BIT = 0x10,
		MPG123_QUIET = 0x20,
		MPG123_GAPLESS = 0x40,
		MPG123_NO_RESYNC = 0x80,
		MPG123_SEEKBUFFER = 0x100,
		MPG123_FUZZY = 0x200,
		MPG123_FORCE_FLOAT = 0x400,
		MPG123_PLAIN_ID3TEXT = 0x800,
		MPG123_IGNORE_STREAMLENGTH = 0x1000
	}

	public enum mpg123_param_rva
	{
		MPG123_RVA_OFF = 0,
		MPG123_RVA_MIX = 1,
		MPG123_RVA_ALBUM = 2,
		MPG123_RVA_MAX = 2
	}

	public enum mpg123_feature_set
	{
		MPG123_FEATURE_ABI_UTF8OPEN,
		MPG123_FEATURE_OUTPUT_8BIT,
		MPG123_FEATURE_OUTPUT_16BIT,
		MPG123_FEATURE_OUTPUT_32BIT,
		MPG123_FEATURE_INDEX,
		MPG123_FEATURE_PARSE_ID3V2,
		MPG123_FEATURE_DECODE_LAYER1,
		MPG123_FEATURE_DECODE_LAYER2,
		MPG123_FEATURE_DECODE_LAYER3,
		MPG123_FEATURE_DECODE_ACCURATE,
		MPG123_FEATURE_DECODE_DOWNSAMPLE,
		MPG123_FEATURE_DECODE_NTOM,
		MPG123_FEATURE_PARSE_ICY,
		MPG123_FEATURE_TIMEOUT_READ
	}

	public enum mpg123_errors
	{
		MPG123_DONE = -12,
		MPG123_NEW_FORMAT = -11,
		MPG123_NEED_MORE = -10,
		MPG123_ERR = -1,
		MPG123_OK = 0,
		MPG123_BAD_OUTFORMAT = 1,
		MPG123_BAD_CHANNEL = 2,
		MPG123_BAD_RATE = 3,
		MPG123_ERR_16TO8TABLE = 4,
		MPG123_BAD_PARAM = 5,
		MPG123_BAD_BUFFER = 6,
		MPG123_OUT_OF_MEM = 7,
		MPG123_NOT_INITIALIZED = 8,
		MPG123_BAD_DECODER = 9,
		MPG123_BAD_HANDLE = 10,
		MPG123_NO_BUFFERS = 11,
		MPG123_BAD_RVA = 12,
		MPG123_NO_GAPLESS = 13,
		MPG123_NO_SPACE = 14,
		MPG123_BAD_TYPES = 0xF,
		MPG123_BAD_BAND = 0x10,
		MPG123_ERR_NULL = 17,
		MPG123_ERR_READER = 18,
		MPG123_NO_SEEK_FROM_END = 19,
		MPG123_BAD_WHENCE = 20,
		MPG123_NO_TIMEOUT = 21,
		MPG123_BAD_FILE = 22,
		MPG123_NO_SEEK = 23,
		MPG123_NO_READER = 24,
		MPG123_BAD_PARS = 25,
		MPG123_BAD_INDEX_PAR = 26,
		MPG123_OUT_OF_SYNC = 27,
		MPG123_RESYNC_FAIL = 28,
		MPG123_NO_8BIT = 29,
		MPG123_BAD_ALIGN = 30,
		MPG123_NULL_BUFFER = 0x1F,
		MPG123_NO_RELSEEK = 0x20,
		MPG123_NULL_POINTER = 33,
		MPG123_BAD_KEY = 34,
		MPG123_NO_INDEX = 35,
		MPG123_INDEX_FAIL = 36,
		MPG123_BAD_DECODER_SETUP = 37,
		MPG123_MISSING_FEATURE = 38,
		MPG123_BAD_VALUE = 39,
		MPG123_LSEEK_FAILED = 40,
		MPG123_BAD_CUSTOM_IO = 41,
		MPG123_LFS_OVERFLOW = 42
	}

	public enum mpg123_enc_enum
	{
		MPG123_ENC_8 = 0xF,
		MPG123_ENC_16 = 0x40,
		MPG123_ENC_32 = 0x100,
		MPG123_ENC_SIGNED = 0x80,
		MPG123_ENC_FLOAT = 3584,
		MPG123_ENC_SIGNED_16 = 208,
		MPG123_ENC_UNSIGNED_16 = 96,
		MPG123_ENC_UNSIGNED_8 = 1,
		MPG123_ENC_SIGNED_8 = 130,
		MPG123_ENC_ULAW_8 = 4,
		MPG123_ENC_ALAW_8 = 8,
		MPG123_ENC_SIGNED_32 = 4480,
		MPG123_ENC_UNSIGNED_32 = 8448,
		MPG123_ENC_FLOAT_32 = 0x200,
		MPG123_ENC_FLOAT_64 = 0x400,
		MPG123_ENC_ANY = 14335
	}

	public enum mpg123_channelcount
	{
		MPG123_MONO = 1,
		MPG123_STEREO
	}

	public enum mpg123_channels
	{
		MPG123_LEFT = 1,
		MPG123_RIGHT,
		MPG123_LR
	}

	public enum mpg123_vbr
	{
		MPG123_CBR,
		MPG123_VBR,
		MPG123_ABR
	}

	public enum mpg123_version
	{
		MPG123_1_0,
		MPG123_2_0,
		MPG123_2_5
	}

	public enum mpg123_mode
	{
		MPG123_M_STEREO,
		MPG123_M_JOINT,
		MPG123_M_DUAL,
		MPG123_M_MONO
	}

	public enum mpg123_flags
	{
		MPG123_CRC = 1,
		MPG123_COPYRIGHT = 2,
		MPG123_PRIVATE = 4,
		MPG123_ORIGINAL = 8
	}

	public enum mpg123_state
	{
		MPG123_ACCURATE = 1
	}

	public enum mpg123_text_encoding
	{
		mpg123_text_unknown = 0,
		mpg123_text_utf8 = 1,
		mpg123_text_latin1 = 2,
		mpg123_text_icy = 3,
		mpg123_text_cp1252 = 4,
		mpg123_text_utf16 = 5,
		mpg123_text_utf16bom = 6,
		mpg123_text_utf16be = 7,
		mpg123_text_max = 7
	}

	public enum mpg123_id3_enc
	{
		mpg123_id3_latin1 = 0,
		mpg123_id3_utf16bom = 1,
		mpg123_id3_utf16be = 2,
		mpg123_id3_utf8 = 3,
		mpg123_id3_enc_max = 3
	}

	public struct mpg123_frameinfo
	{
		private mpg123_version version;

		private int layer;

		private int rate;

		private mpg123_mode mode;

		private int mode_ext;

		private int framesize;

		private mpg123_flags flags;

		private int emphasis;

		private int bitrate;

		private int abr_rate;

		private mpg123_vbr vbr;
	}

	public struct mpg123_string
	{
		private string p;

		private int size;

		private int fill;
	}

	public struct mpg123_text
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private char[] lang;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private char[] id;

		private mpg123_string description;

		private mpg123_string text;
	}

	public struct mpg123_id3v2
	{
		private byte version;

		private IntPtr title;

		private IntPtr artist;

		private IntPtr album;

		private IntPtr year;

		private IntPtr genre;

		private IntPtr comment;

		private IntPtr comment_list;

		private int comments;

		private IntPtr text;

		private int texts;

		private IntPtr extra;

		private int extras;
	}

	public struct mpg123_id3v1
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public char[] tag;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
		public char[] title;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
		public char[] artist;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
		public char[] album;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public char[] year;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
		public char[] comment;

		public byte genre;
	}

	private const string Mpg123Dll = "libmpg123-0";

	[DllImport("libmpg123-0")]
	public static extern int mpg123_init();

	[DllImport("libmpg123-0")]
	public static extern void mpg123_exit();

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern IntPtr mpg123_new(string decoder, IntPtr error);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_delete(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_param(IntPtr mh, mpg123_parms type, int value, double fvalue);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_getparam(IntPtr mh, mpg123_parms type, IntPtr val, IntPtr fval);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_feature(mpg123_feature_set key);

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern string mpg123_plain_strerror(int errcode);

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern string mpg123_strerror(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_errcode(IntPtr mh);

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern IntPtr mpg123_decoders();

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern IntPtr mpg123_supported_decoders();

	[DllImport("libmpg123-0")]
	public static extern int mpg123_decoder(IntPtr mh, string decoder_name);

	[DllImport("libmpg123-0")]
	public static extern string mpg123_current_decoder(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_rates(IntPtr list, IntPtr number);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_encodings(IntPtr list, IntPtr number);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_format_none(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_format_all(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_format(IntPtr mh, int rate, int channels, int encodings);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_format_support(IntPtr mh, int rate, int encoding);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_getformat(IntPtr mh, out IntPtr rate, out IntPtr channels, out IntPtr encoding);

	[DllImport("libmpg123-0", CharSet = CharSet.Ansi)]
	public static extern int mpg123_open(IntPtr mh, string path);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_open_fd(IntPtr mh, int fd);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_open_handle(IntPtr mh, IntPtr iohandle);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_open_feed(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_close(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_read(IntPtr mh, byte[] outmemory, int outmemsize, out IntPtr done);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_feed(IntPtr mh, IntPtr input, int size);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_decode(IntPtr mh, IntPtr inmemory, int inmemsize, IntPtr outmemory, int outmemsize, IntPtr done);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_decode_frame(IntPtr mh, IntPtr num, IntPtr audio, IntPtr bytes);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_framebyframe_decode(IntPtr mh, IntPtr num, IntPtr audio, IntPtr bytes);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_framebyframe_next(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_tell(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_tellframe(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_tell_stream(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_seek(IntPtr mh, int sampleoff, int whence);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_feedseek(IntPtr mh, int sampleoff, int whence, IntPtr input_offset);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_seek_frame(IntPtr mh, int frameoff, int whence);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_timeframe(IntPtr mh, double sec);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_index(IntPtr mh, IntPtr offsets, IntPtr step, IntPtr fill);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_set_index(IntPtr mh, IntPtr offsets, int step, int fill);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_position(IntPtr mh, int frame_offset, int buffered_bytes, IntPtr current_frame, IntPtr frames_left, IntPtr current_seconds, IntPtr seconds_left);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_eq(IntPtr mh, mpg123_channels channel, int band, double val);

	[DllImport("libmpg123-0")]
	public static extern double mpg123_geteq(IntPtr mh, mpg123_channels channel, int band);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_reset_eq(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_volume(IntPtr mh, double vol);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_volume_change(IntPtr mh, double change);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_getvolume(IntPtr mh, IntPtr _base, IntPtr really, IntPtr rva_db);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_info(IntPtr mh, IntPtr mi);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_safe_buffer();

	[DllImport("libmpg123-0")]
	public static extern int mpg123_scan(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_length(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_set_filesize(IntPtr mh, int size);

	[DllImport("libmpg123-0")]
	public static extern double mpg123_tpf(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_clip(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_getstate(IntPtr mh, mpg123_state key, IntPtr val, IntPtr fval);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_init_string(IntPtr sb);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_free_string(IntPtr sb);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_resize_string(IntPtr sb, int news);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_grow_string(IntPtr sb, int news);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_copy_string(IntPtr from, IntPtr to);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_add_string(IntPtr sb, string stuff);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_add_substring(IntPtr sb, string stuff, int from, int count);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_set_string(IntPtr sb, string stuff);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_set_substring(IntPtr sb, string stuff, int from, int count);

	[DllImport("libmpg123-0")]
	public static extern mpg123_text_encoding mpg123_enc_from_id3(byte id3_enc_byte);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_store_utf8(IntPtr sb, mpg123_text_encoding enc, string source, int source_size);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_meta_check(IntPtr mh);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_id3(IntPtr mh, out IntPtr v1, out IntPtr v2);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_icy(IntPtr mh, IntPtr icy_meta);

	[DllImport("libmpg123-0")]
	public static extern string mpg123_icy2utf8(string icy_text);

	[DllImport("libmpg123-0")]
	public static extern IntPtr mpg123_parnew(IntPtr mp, string decoder, IntPtr error);

	[DllImport("libmpg123-0")]
	public static extern IntPtr mpg123_new_pars(IntPtr error);

	[DllImport("libmpg123-0")]
	public static extern void mpg123_delete_pars(IntPtr mp);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_fmt_none(IntPtr mp);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_fmt_all(IntPtr mp);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_fmt(IntPtr mh, int rate, int channels, int encodings);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_fmt_support(IntPtr mh, int rate, int encoding);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_par(IntPtr mp, mpg123_parms type, int value, double fvalue);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_getpar(IntPtr mp, mpg123_parms type, IntPtr val, IntPtr fval);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_replace_buffer(IntPtr mh, string data, int size);

	[DllImport("libmpg123-0")]
	public static extern int mpg123_outblock(IntPtr mh);
}
