using System;
using System.IO;
using System.Text;

namespace Sentry.KeyChip
{
    /// <summary>
    /// Keychip用アプリケーションバイナリデータ処理クラス
    /// </summary>
    public class KeyChipData
    {
        private KeyChipData() { }

        /// <summary>
        /// メタデータ
        /// </summary>
        public AppInfo Info { get; set; }

        /// <summary>
        /// 暗号データ
        /// </summary>
        public AppData Data { get; set; }

        public enum ModelType : byte
        {
            Server = 1,
            Satellite = 2,
            Live = 3,
            Terminal = 4
        }

        [Flags]
        public enum RegionFlag : byte
        {
            None = 0,       // 00000000
            Japan = 1,      // 00000001 JPN ( 日本 )
            USA = 2,        // 00000010 USA ( 米国 )
            Export = 4,     // 00000100 EXP ( 外国 )
            China = 8,      // 00001000 CHN ( 中国 )
            // Global = 14, // 00001110 GLB ( 日本以外 )
            Flag4 = 16,     // 00010000 ※ 使用しない
            Flag5 = 32,     // 00100000 ※ 使用しない
            Flag6 = 64,     // 01000000 ※ 使用しない
            Flag7 = 128,    // 10000000 ※ 使用しない
            // All = 255    // 11111111 ALL ( すべて対応 )
        }

        [Flags]
        public enum SystemFlag : byte
        {
            None = 0,           // 00000000
            DevType = 1,        // 00000001 develop-flag ( 開発 フラグ )
            Binding = 2,        // 00000010 binding-flag ( 紐付け ) ※ 使用しないとする
            AllNetType = 4,     // 00000100 allnet-flag  ( ALL.Net Auth フラグ )
            LanServer = 8,      // 00001000 deliver-flag ( LAN インストール )
            AllNetSlim = 16,    // 00010000 authrtl-flag ( ALL.Net Slim フラグ )
            BillingTitle = 32,  // 00100000 billing-flag ( ALL.Net 課金 フラグ )
            BillingType = 64,   // 01000000 rental-flag  ( P-ras 課金 フラグ )
            Flag7 = 128         // 10000000 ※ 使用しない
        }

        public class AppInfo
        {
            /// <summary>
            /// オブジェクト内のバイト数
            /// </summary>
            public const int Length = 0x30;

            /// <summary>
            /// ファイルのCRC値
            /// </summary>
            public uint Checksum { get; private set; }

            /// <summary>
            /// フォーマットタイプ
            /// </summary>
            public byte FormatType { get; private set; }

            /// <summary>
            /// ゲームID
            /// </summary>
            public string GameId { get; private set; }

            /// <summary>
            /// 対応地域
            /// </summary>
            public byte RegionId { get; private set; }

            /// <summary>
            /// 機能区分
            /// </summary>
            public byte ModelTypeId { get; private set; }

            /// <summary>
            /// システムフラグ
            /// </summary>
            public byte SystemFlagId { get; private set; }

            /// <summary>
            /// 課金フラグ
            /// </summary>
            public byte BillingMode { get; private set; }

            /// <summary>
            /// プラットフォームID
            /// </summary>
            public string PlatformId { get; private set; }

            /// <summary>
            /// DVDフラグ ( ELEFUN では使用しない )
            /// </summary>
            public byte DvdFlag { get; private set; }

            /// <summary>
            /// 対応ネットワークアドレス
            /// </summary>
            public byte[] NetworkAddr { get; private set; }

            /// <summary>
            /// リージョンフラグの変換
            /// </summary>
            public RegionFlag RegionFlag
            {
                get
                {
                    return (RegionFlag)RegionId;
                }
            }

            /// <summary>
            /// 機能区分の変換
            /// </summary>
            public ModelType ModelType
            {
                get
                {
                    return (ModelType)ModelTypeId;
                }
            }

            /// <summary>
            /// システムフラグの変換
            /// </summary>
            public SystemFlag SystemFlag
            {
                get
                {
                    return (SystemFlag)SystemFlagId;
                }
            }

            /// <summary>
            /// リージョン名を取得
            /// </summary>
            public string RegionName
            {
                get
                {
                    switch (RegionId)
                    {
                        case 0: return "--";
                        case 1: return "JP";
                        case 2: return "US";
                        case 4: return "EX";
                        case 8: return "CN";
                        case 14: return "GL";
                        default: return "--";
                    }
                }
            }

            /// <summary>
            /// 機能区分を取得
            /// </summary>
            public string ModelName
            {
                get
                {
                    switch (ModelTypeId)
                    {
                        case 0: return "--";
                        case 1: return "SV";
                        case 2: return "ST";
                        case 3: return "LV";
                        case 4: return "TM";
                        default: return "--";
                    }
                }
            }

            /// <summary>
            /// ラベル印字に使われる文字列
            /// </summary>
            public string LabelName
            {
                get
                {
                    return String.Format("{0} {1} {2}", GameId, ModelName, RegionName);
                }
            }

            /// <summary>
            /// 内部データに解析した値をセット
            /// </summary>
            private void Deserialize(byte[] data)
            {
                using (var stream = new MemoryStream(data))
                using (var reader = new BinaryReader(stream))
                {
                    Checksum = reader.ReadUInt32();
                    FormatType = reader.ReadByte();
                    reader.ReadBytes(3); // Padding
                    GameId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    RegionId = reader.ReadByte();
                    ModelTypeId = reader.ReadByte();
                    SystemFlagId = reader.ReadByte();
                    BillingMode = reader.ReadByte();
                    PlatformId = Encoding.ASCII.GetString(reader.ReadBytes(3));
                    DvdFlag = reader.ReadByte();
                    NetworkAddr = reader.ReadBytes(4);
                }
            }

            public static AppInfo Parse(byte[] data)
            {
                var obj = new AppInfo();
                obj.Deserialize(data);
                return obj;
            }
        }

        public abstract class AppData
        {
            protected AppData() { }

            /// <summary>
            /// オブジェクト内のバイト数
            /// </summary>
            public int Length
            {
                get
                {
                    return GetLength();
                }
            }

            public byte[] Seed { get; set; }

            public byte[] Key { get; set; }

            public byte[] IV { get; set; }

            protected abstract int GetLength();

            protected abstract void Deserialize(byte[] data);

            public static AppData Parse<T>(byte[] data) where T : AppData
            {
                var obj = Activator.CreateInstance<T>();
                obj.Deserialize(data);
                return obj;
            }
        }

        public class RingAppData : AppData
        {
            private RingAppData() { }

            protected override int GetLength()
            {
                return 0xB0;
            }

            /// <summary>
            /// 内部データに解析した値をセット
            /// </summary>
            protected override void Deserialize(byte[] data)
            {
                using (var stream = new MemoryStream(data))
                using (var reader = new BinaryReader(stream))
                {
                    Seed = reader.ReadBytes(16);
                    Key = reader.ReadBytes(16);
                    IV = reader.ReadBytes(16);
                }
            }

            public static AppData Parse(byte[] data)
            {
                var obj = new RingAppData();
                obj.Deserialize(data);
                return obj;
            }
        }

        public class NuAppData : AppData
        {
            private NuAppData() { }

            protected override int GetLength()
            {
                return 0x40;
            }

            /// <summary>
            /// 内部データに解析した値をセット
            /// </summary>
            protected override void Deserialize(byte[] data)
            {
                using (var stream = new MemoryStream(data))
                using (var reader = new BinaryReader(stream))
                {
                    Key = reader.ReadBytes(16);
                    IV = reader.ReadBytes(16);
                    Seed = reader.ReadBytes(32);
                }
            }

            public static AppData Parse(byte[] data)
            {
                var obj = new NuAppData();
                obj.Deserialize(data);
                return obj;
            }
        }

        /// <summary>
        /// 内部データに解析した値をセット
        /// </summary>
        private void Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var isRing = stream.Length > 0x70;
                Info = AppInfo.Parse(reader.ReadBytes(AppInfo.Length));
                if (isRing)
                {
                    stream.Seek(240, SeekOrigin.Begin);
                    var remaining = stream.Length - stream.Position;
                    Data = RingAppData.Parse(reader.ReadBytes((int)remaining));
                }
                else
                {
                    var remaining = stream.Length - stream.Position;
                    Data = NuAppData.Parse(reader.ReadBytes((int)remaining));
                }
            }
        }

        public static KeyChipData Parse(byte[] data)
        {
            var obj = new KeyChipData();
            obj.Deserialize(data);
            return obj;
        }
    }
}
