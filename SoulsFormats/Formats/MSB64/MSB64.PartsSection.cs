﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB64
    {
        public class PartsSection : Section<Part>
        {
            /// <summary>
            /// The MSB type string for this section.
            /// </summary>
            public override string Type => "PARTS_PARAM_ST";

            public List<Part.MapPiece> MapPieces;

            public List<Part.Object> Objects;

            public List<Part.Enemy> Enemies;

            public List<Part.Player> Players;

            public List<Part.Collision> Collisions;

            public List<Part.DummyObject> DummyObjects;

            public List<Part.DummyEnemy> DummyEnemies;

            public List<Part.ConnectCollision> ConnectCollisions;

            internal PartsSection(BinaryReaderEx br, int unk1) : base(br, unk1)
            {
                MapPieces = new List<Part.MapPiece>();
                Objects = new List<Part.Object>();
                Enemies = new List<Part.Enemy>();
                Players = new List<Part.Player>();
                Collisions = new List<Part.Collision>();
                DummyObjects = new List<Part.DummyObject>();
                DummyEnemies = new List<Part.DummyEnemy>();
                ConnectCollisions = new List<Part.ConnectCollision>();
            }

            internal override List<Part> GetEntries()
            {
                return Util.ConcatAll<Part>(
                    MapPieces, Objects, Enemies, Players, Collisions, DummyObjects, DummyEnemies, ConnectCollisions);
            }

            internal override Part ReadEntry(BinaryReaderEx br)
            {
                PartsType type = br.GetEnum32<PartsType>(br.Position + 8);

                switch (type)
                {
                    case PartsType.MapPiece:
                        var mapPiece = new Part.MapPiece(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case PartsType.Object:
                        var obj = new Part.Object(br);
                        Objects.Add(obj);
                        return obj;

                    case PartsType.Enemy:
                        var enemy = new Part.Enemy(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case PartsType.Player:
                        var player = new Part.Player(br);
                        Players.Add(player);
                        return player;

                    case PartsType.Collision:
                        var collision = new Part.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    case PartsType.DummyObject:
                        var dummyObj = new Part.DummyObject(br);
                        DummyObjects.Add(dummyObj);
                        return dummyObj;

                    case PartsType.DummyEnemy:
                        var dummyEne = new Part.DummyEnemy(br);
                        DummyEnemies.Add(dummyEne);
                        return dummyEne;

                    case PartsType.ConnectCollision:
                        var connectColl = new Part.ConnectCollision(br);
                        ConnectCollisions.Add(connectColl);
                        return connectColl;

                    default:
                        throw new NotImplementedException($"Unsupported part type: {type}");
                }
            }

            internal override void WriteEntries(BinaryWriterEx bw, List<Part> entries)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    bw.FillInt64($"Offset{i}", bw.Position);
                    entries[i].Write(bw);
                }
            }

            internal void GetNames(MSB64 msb, Entries entries)
            {
                foreach (Part part in entries.Parts)
                    part.GetNames(msb, entries);
            }

            internal void GetIndices(MSB64 msb, Entries entries)
            {
                foreach (Part part in entries.Parts)
                    part.GetIndices(msb, entries);
            }
        }

        public enum PartsType : uint
        {
            MapPiece = 0x0,
            Object = 0x1,
            Enemy = 0x2,
            Item = 0x3,
            Player = 0x4,
            Collision = 0x5,
            NPCWander = 0x6,
            Protoboss = 0x7,
            Navmesh = 0x8,
            DummyObject = 0x9,
            DummyEnemy = 0xA,
            ConnectCollision = 0xB,
        }

        public abstract class Part : Entry
        {
            internal abstract PartsType Type { get; }

            public override string Name { get; set; }
            public string Placeholder;
            public int ID;
            private int modelIndex;
            public string ModelName;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public uint DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4;
            public uint DispGroup1, DispGroup2, DispGroup3, DispGroup4;
            public int UnkF01, UnkF02, UnkF03, UnkF04, UnkF05, UnkF06, UnkF07, UnkF08, UnkF09,
                UnkF10, UnkF11, UnkF12, UnkF13, UnkF14, UnkF15, UnkF16, UnkF17, UnkF18;

            public int EventEntityID;
            public byte LightID, FogID, ScatterID, Unk07;
            public int UnkB0C, UnkB10, UnkB14, UnkB18, UnkB1C, UnkB20, UnkB24, UnkB28, UnkB30, UnkB34, UnkB38;
            public long UnkOffset1Delta, UnkOffset2Delta;

            public Part(Part clone)
            {
                Name = clone.Name;
                Placeholder = clone.Placeholder;
                ID = clone.ID;
                ModelName = clone.ModelName;
                Position = new Vector3(clone.Position.X, clone.Position.Y, clone.Position.Z);
                Rotation = new Vector3(clone.Rotation.X, clone.Rotation.Y, clone.Rotation.Z);
                Scale = new Vector3(clone.Scale.X, clone.Scale.Y, clone.Scale.Z);
                DrawGroup1 = clone.DrawGroup1;
                DrawGroup2 = clone.DrawGroup2;
                DrawGroup3 = clone.DrawGroup3;
                DrawGroup4 = clone.DrawGroup4;
                DispGroup1 = clone.DispGroup1;
                DispGroup2 = clone.DispGroup2;
                DispGroup3 = clone.DispGroup3;
                DispGroup4 = clone.DispGroup4;
                UnkF01 = clone.UnkF01;
                UnkF02 = clone.UnkF02;
                UnkF03 = clone.UnkF03;
                UnkF04 = clone.UnkF04;
                UnkF05 = clone.UnkF05;
                UnkF06 = clone.UnkF06;
                UnkF07 = clone.UnkF07;
                UnkF08 = clone.UnkF08;
                UnkF09 = clone.UnkF09;
                UnkF10 = clone.UnkF10;
                UnkF11 = clone.UnkF11;
                UnkF12 = clone.UnkF12;
                UnkF13 = clone.UnkF13;
                UnkF14 = clone.UnkF14;
                UnkF15 = clone.UnkF15;
                UnkF16 = clone.UnkF16;
                UnkF17 = clone.UnkF17;
                UnkF18 = clone.UnkF18;
                EventEntityID = clone.EventEntityID;
                LightID = clone.LightID;
                FogID = clone.FogID;
                ScatterID = clone.ScatterID;
                Unk07 = clone.Unk07;
                UnkB0C = clone.UnkB0C;
                UnkB10 = clone.UnkB10;
                UnkB14 = clone.UnkB14;
                UnkB18 = clone.UnkB18;
                UnkB1C = clone.UnkB1C;
                UnkB20 = clone.UnkB20;
                UnkB24 = clone.UnkB24;
                UnkB28 = clone.UnkB28;
                UnkB30 = clone.UnkB30;
                UnkB34 = clone.UnkB34;
                UnkB38 = clone.UnkB38;
                UnkOffset1Delta = clone.UnkOffset1Delta;
                UnkOffset2Delta = clone.UnkOffset2Delta;
            }

            internal Part(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                ID = br.ReadInt32();
                modelIndex = br.ReadInt32();
                br.AssertInt32(0);
                long placeholderOffset = br.ReadInt64();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();

                DrawGroup1 = br.ReadUInt32();
                DrawGroup2 = br.ReadUInt32();
                DrawGroup3 = br.ReadUInt32();
                DrawGroup4 = br.ReadUInt32();
                DispGroup1 = br.ReadUInt32();
                DispGroup2 = br.ReadUInt32();
                DispGroup3 = br.ReadUInt32();
                DispGroup4 = br.ReadUInt32();

                UnkF01 = br.ReadInt32();
                UnkF02 = br.ReadInt32();
                UnkF03 = br.ReadInt32();
                UnkF04 = br.ReadInt32();
                UnkF05 = br.ReadInt32();
                UnkF06 = br.ReadInt32();
                UnkF07 = br.ReadInt32();
                UnkF08 = br.ReadInt32();
                UnkF09 = br.ReadInt32();
                UnkF10 = br.ReadInt32();
                UnkF11 = br.ReadInt32();
                UnkF12 = br.ReadInt32();
                UnkF13 = br.ReadInt32();
                UnkF14 = br.ReadInt32();
                UnkF15 = br.ReadInt32();
                UnkF16 = br.ReadInt32();
                UnkF17 = br.ReadInt32();
                UnkF18 = br.ReadInt32();
                br.AssertInt32(0);

                long baseDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();
                UnkOffset1Delta = br.ReadInt64();
                if (UnkOffset1Delta != 0)
                    UnkOffset1Delta -= typeDataOffset;
                UnkOffset2Delta = br.ReadInt64();
                if (UnkOffset2Delta != 0)
                    UnkOffset2Delta -= typeDataOffset;

                Name = br.GetUTF16(start + nameOffset);
                if (placeholderOffset == 0)
                    Placeholder = null;
                else
                    Placeholder = br.GetUTF16(start + placeholderOffset);

                br.StepIn(start + baseDataOffset);
                EventEntityID = br.ReadInt32();

                LightID = br.ReadByte();
                FogID = br.ReadByte();
                ScatterID = br.ReadByte();
                Unk07 = br.ReadByte();

                br.AssertInt32(0);
                UnkB0C = br.ReadInt32();
                UnkB10 = br.ReadInt32();
                UnkB14 = br.ReadInt32();
                UnkB18 = br.ReadInt32();
                UnkB1C = br.ReadInt32();
                UnkB20 = br.ReadInt32();
                UnkB24 = br.ReadInt32();
                UnkB28 = br.ReadInt32();
                br.AssertInt32(-1);
                UnkB30 = br.ReadInt32();
                UnkB34 = br.ReadInt32();
                UnkB38 = br.ReadInt32();
                br.AssertInt32(0);
                br.StepOut();

                br.StepIn(start + typeDataOffset);
                Read(br);
                br.StepOut();
            }

            internal abstract void Read(BinaryReaderEx br);

            internal void Write(BinaryWriterEx bw)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(ID);
                bw.WriteInt32(modelIndex);
                bw.WriteInt32(0);
                bw.ReserveInt64("PlaceholderOffset");
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);

                bw.WriteUInt32(DrawGroup1);
                bw.WriteUInt32(DrawGroup2);
                bw.WriteUInt32(DrawGroup3);
                bw.WriteUInt32(DrawGroup4);
                bw.WriteUInt32(DispGroup1);
                bw.WriteUInt32(DispGroup2);
                bw.WriteUInt32(DispGroup3);
                bw.WriteUInt32(DispGroup4);

                bw.WriteInt32(UnkF01);
                bw.WriteInt32(UnkF02);
                bw.WriteInt32(UnkF03);
                bw.WriteInt32(UnkF04);
                bw.WriteInt32(UnkF05);
                bw.WriteInt32(UnkF06);
                bw.WriteInt32(UnkF07);
                bw.WriteInt32(UnkF08);
                bw.WriteInt32(UnkF09);
                bw.WriteInt32(UnkF10);
                bw.WriteInt32(UnkF11);
                bw.WriteInt32(UnkF12);
                bw.WriteInt32(UnkF13);
                bw.WriteInt32(UnkF14);
                bw.WriteInt32(UnkF15);
                bw.WriteInt32(UnkF16);
                bw.WriteInt32(UnkF17);
                bw.WriteInt32(UnkF18);
                bw.WriteInt32(0);

                bw.ReserveInt64("BaseDataOffset");
                bw.ReserveInt64("TypeDataOffset");
                bw.ReserveInt64("UnkOffset1");
                bw.ReserveInt64("UnkOffset2");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                if (Placeholder == null)
                    bw.FillInt64("PlaceholderOffset", 0);
                else
                {
                    bw.FillInt64("PlaceholderOffset", bw.Position - start);
                    bw.WriteUTF16(Placeholder, true);
                }
                bw.Pad(8);

                bw.FillInt64("BaseDataOffset", bw.Position - start);
                bw.WriteInt32(EventEntityID);

                bw.WriteByte(LightID);
                bw.WriteByte(FogID);
                bw.WriteByte(ScatterID);
                bw.WriteByte(Unk07);

                bw.WriteInt32(0);
                bw.WriteInt32(UnkB0C);
                bw.WriteInt32(UnkB10);
                bw.WriteInt32(UnkB14);
                bw.WriteInt32(UnkB18);
                bw.WriteInt32(UnkB1C);
                bw.WriteInt32(UnkB20);
                bw.WriteInt32(UnkB24);
                bw.WriteInt32(UnkB28);
                bw.WriteInt32(-1);
                bw.WriteInt32(UnkB30);
                bw.WriteInt32(UnkB34);
                bw.WriteInt32(UnkB38);
                bw.WriteInt32(0);

                bw.FillInt64("TypeDataOffset", bw.Position - start);
                if (UnkOffset1Delta == 0)
                    bw.FillInt64("UnkOffset1", 0);
                else
                    bw.FillInt64("UnkOffset1", bw.Position - start + UnkOffset1Delta);

                if (UnkOffset2Delta == 0)
                    bw.FillInt64("UnkOffset2", 0);
                else
                    bw.FillInt64("UnkOffset2", bw.Position - start + UnkOffset2Delta);

                WriteSpecific(bw);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw);

            internal virtual void GetNames(MSB64 msb, Entries entries)
            {
                ModelName = GetName(entries.Models, modelIndex);
            }

            internal virtual void GetIndices(MSB64 msb, Entries entries)
            {
                modelIndex = GetIndex(entries.Models, ModelName);
            }

            public override string ToString()
            {
                return $"{Type} {ID} : {Name}";
            }

            public class MapPiece : Part
            {
                internal override PartsType Type => PartsType.MapPiece;

                public int UnkT01, UnkT02, UnkT03, UnkT04;

                internal MapPiece(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT01 = br.ReadInt32();
                    UnkT02 = br.ReadInt32();
                    UnkT03 = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT01);
                    bw.WriteInt32(UnkT02);
                    bw.WriteInt32(UnkT03);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            public class Object : Part
            {
                internal override PartsType Type => PartsType.Object;

                private int collisionPartIndex;
                public string CollisionName;

                public int UnkT04, UnkT06, UnkT07, UnkT08, UnkT09, UnkT10;

                public short UnkT02a, UnkT02b, UnkT03a, UnkT03b, UnkT05a, UnkT05b;

                public Object(Object clone) : base(clone)
                {
                    CollisionName = clone.CollisionName;
                    UnkT02a = clone.UnkT02a;
                    UnkT02b = clone.UnkT02b;
                    UnkT03a = clone.UnkT03a;
                    UnkT03b = clone.UnkT03b;
                    UnkT04 = clone.UnkT04;
                    UnkT05a = clone.UnkT05a;
                    UnkT05b = clone.UnkT05b;
                    UnkT06 = clone.UnkT06;
                    UnkT07 = clone.UnkT07;
                    UnkT08 = clone.UnkT08;
                    UnkT09 = clone.UnkT09;
                    UnkT10 = clone.UnkT10;
                }

                internal Object(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    collisionPartIndex = br.ReadInt32();
                    UnkT02a = br.ReadInt16();
                    UnkT02b = br.ReadInt16();
                    UnkT03a = br.ReadInt16();
                    UnkT03b = br.ReadInt16();
                    UnkT04 = br.ReadInt32();
                    UnkT05a = br.ReadInt16();
                    UnkT05b = br.ReadInt16();
                    UnkT06 = br.ReadInt32();
                    UnkT07 = br.ReadInt32();
                    UnkT08 = br.ReadInt32();
                    UnkT09 = br.ReadInt32();
                    UnkT10 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(collisionPartIndex);
                    bw.WriteInt16(UnkT02a);
                    bw.WriteInt16(UnkT02b);
                    bw.WriteInt16(UnkT03a);
                    bw.WriteInt16(UnkT03b);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt16(UnkT05a);
                    bw.WriteInt16(UnkT05b);
                    bw.WriteInt32(UnkT06);
                    bw.WriteInt32(UnkT07);
                    bw.WriteInt32(UnkT08);
                    bw.WriteInt32(UnkT09);
                    bw.WriteInt32(UnkT10);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB64 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(entries.Parts, collisionPartIndex);
                }

                internal override void GetIndices(MSB64 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionPartIndex = GetIndex(entries.Parts, CollisionName);
                }
            }

            public class Enemy : Part
            {
                internal override PartsType Type => PartsType.Enemy;

                private int collisionPartIndex;
                public string CollisionName;
                public int ThinkParamID, NPCParamID, TalkID, UnkT04, CharaInitID, UnkT07, UnkT08, UnkT09;
                public float UnkT10;
                public int UnkT11, UnkT12, UnkT13, UnkT14, UnkT15, UnkT16, UnkT17, UnkT18, UnkT19;

                public Enemy(Enemy clone) : base(clone)
                {
                    ThinkParamID = clone.ThinkParamID;
                    NPCParamID = clone.NPCParamID;
                    TalkID = clone.TalkID;
                    UnkT04 = clone.UnkT04;
                    CharaInitID = clone.CharaInitID;
                    CollisionName = clone.CollisionName;
                    UnkT07 = clone.UnkT07;
                    UnkT08 = clone.UnkT08;
                    UnkT09 = clone.UnkT09;
                    UnkT10 = clone.UnkT10;
                    UnkT11 = clone.UnkT11;
                    UnkT12 = clone.UnkT12;
                    UnkT13 = clone.UnkT13;
                    UnkT14 = clone.UnkT14;
                    UnkT15 = clone.UnkT15;
                    UnkT16 = clone.UnkT16;
                    UnkT17 = clone.UnkT17;
                    UnkT18 = clone.UnkT18;
                    UnkT19 = clone.UnkT19;
                }

                internal Enemy(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ThinkParamID = br.ReadInt32();
                    NPCParamID = br.ReadInt32();
                    TalkID = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    CharaInitID = br.ReadInt32();
                    collisionPartIndex = br.ReadInt32();
                    UnkT07 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    UnkT08 = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT09 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT10 = br.ReadSingle();
                    br.AssertInt32(-1);
                    UnkT11 = br.ReadInt32();
                    br.AssertInt32(-1);
                    UnkT12 = br.ReadInt32();
                    br.AssertInt32(-1);
                    UnkT13 = br.ReadInt32();
                    br.AssertInt32(-1);
                    UnkT14 = br.ReadInt32();
                    br.AssertInt32(-1);
                    UnkT15 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT16 = br.ReadInt32();
                    UnkT17 = br.ReadInt32();
                    UnkT18 = br.ReadInt32();
                    UnkT19 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ThinkParamID);
                    bw.WriteInt32(NPCParamID);
                    bw.WriteInt32(TalkID);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(CharaInitID);
                    bw.WriteInt32(collisionPartIndex);
                    bw.WriteInt32(UnkT07);
                    bw.WriteInt32(0);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT08);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT09);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteSingle(UnkT10);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT11);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT12);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT13);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT14);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT15);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT16);
                    bw.WriteInt32(UnkT17);
                    bw.WriteInt32(UnkT18);
                    bw.WriteInt32(UnkT19);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB64 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(entries.Parts, collisionPartIndex);
                }

                internal override void GetIndices(MSB64 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionPartIndex = GetIndex(entries.Parts, CollisionName);
                }
            }

            public class Player : Part
            {
                internal override PartsType Type => PartsType.Player;

                internal Player(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            public class Collision : Part
            {
                internal override PartsType Type => PartsType.Collision;

                public int DisableBonfireEntityID, PlayRegionID;

                public int UnkT00, UnkT04, UnkT2C, UnkT30, UnkT34,
                    UnkT3C, UnkT50, UnkT54, UnkT58, UnkT5C, UnkT74;

                public short UnkT24, UnkT26;

                public float UnkT78;

                internal Collision(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertInt32(0); // Navmesh Group (4)
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(-1); // Vagrant Entity ID (3)
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    UnkT24 = br.ReadInt16();
                    UnkT26 = br.AssertInt16(0, 1);
                    DisableBonfireEntityID = br.ReadInt32();
                    UnkT2C = br.ReadInt32();
                    UnkT30 = br.ReadInt32();
                    UnkT34 = br.ReadInt32();
                    PlayRegionID = br.ReadInt32();
                    UnkT3C = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT50 = br.ReadInt32();
                    UnkT54 = br.ReadInt32();
                    UnkT58 = br.ReadInt32();
                    UnkT5C = br.ReadInt32();

                    for (int i = 0; i < 19; i++)
                        br.AssertInt32(0);

                    UnkT74 = br.ReadInt32();
                    UnkT78 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt16(UnkT24);
                    bw.WriteInt16(UnkT26);
                    bw.WriteInt32(DisableBonfireEntityID);
                    bw.WriteInt32(UnkT2C);
                    bw.WriteInt32(UnkT30);
                    bw.WriteInt32(UnkT34);
                    bw.WriteInt32(PlayRegionID);
                    bw.WriteInt32(UnkT3C);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT50);
                    bw.WriteInt32(UnkT54);
                    bw.WriteInt32(UnkT58);
                    bw.WriteInt32(UnkT5C);

                    for (int i = 0; i < 19; i++)
                        bw.WriteInt32(0);

                    bw.WriteInt32(UnkT74);
                    bw.WriteSingle(UnkT78);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            public class DummyObject : Object
            {
                internal override PartsType Type => PartsType.DummyObject;

                internal DummyObject(BinaryReaderEx br) : base(br) { }
            }

            public class DummyEnemy : Enemy
            {
                internal override PartsType Type => PartsType.DummyEnemy;

                internal DummyEnemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Determines which collision parts load other maps.
            /// </summary>
            public class ConnectCollision : Part
            {
                internal override PartsType Type => PartsType.ConnectCollision;

                private int collisionIndex;

                /// <summary>
                /// The name of the associated collision part.
                /// </summary>
                public string CollisionName;

                /// <summary>
                /// A map ID in format mXX_XX_XX_XX.
                /// </summary>
                public byte MapID1, MapID2, MapID3, MapID4;

                internal ConnectCollision(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    collisionIndex = br.ReadInt32();
                    MapID1 = br.ReadByte();
                    MapID2 = br.ReadByte();
                    MapID3 = br.ReadByte();
                    MapID4 = br.ReadByte();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(collisionIndex);
                    bw.WriteByte(MapID1);
                    bw.WriteByte(MapID2);
                    bw.WriteByte(MapID3);
                    bw.WriteByte(MapID4);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB64 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(msb.Parts.Collisions, collisionIndex);
                }

                internal override void GetIndices(MSB64 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionIndex = GetIndex(msb.Parts.Collisions, CollisionName);
                }
            }
        }
    }
}
