﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Confuser.Core.Poly;
using System.IO;
using Mono.Cecil.Cil;
using System.IO.Compression;
using Mono.Cecil.Rocks;
using Confuser.Core.Poly.Visitors;
using System.Collections.Specialized;
using Mono.Cecil.Metadata;

namespace Confuser.Core.Confusions
{
    public class ConstantConfusion : IConfusion
    {
        class Phase1 : StructurePhase
        {
            public Phase1(ConstantConfusion cc) { this.cc = cc; }
            ConstantConfusion cc;
            public override IConfusion Confusion
            {
                get { return cc; }
            }

            public override int PhaseID
            {
                get { return 1; }
            }

            public override Priority Priority
            {
                get { return Priority.CodeLevel; }
            }

            public override bool WholeRun
            {
                get { return true; }
            }

            public override void Initialize(ModuleDefinition mod)
            {
                this.mod = mod;
                cc.txts[mod] = new _Context();

                cc.txts[mod].dats = new List<Data>();
                cc.txts[mod].idx = 0;
                cc.txts[mod].dict = new Dictionary<object, int>();
            }

            public override void DeInitialize()
            {
                //
            }

            ModuleDefinition mod;
            public override void Process(ConfusionParameter parameter)
            {
                if (parameter.GlobalParameters["type"] != "dynamic" &&
                    parameter.GlobalParameters["type"] != "native")
                {
                    ProcessSafe(parameter); return;
                }
                _Context txt = cc.txts[mod];
                txt.isNative = parameter.GlobalParameters["type"] == "native";

                Random rand = new Random();
                TypeDefinition modType = mod.GetType("<Module>");

                AssemblyDefinition id = AssemblyDefinition.ReadAssembly(typeof(Iid).Assembly.Location);
                txt.strer = id.MainModule.GetType("Encryptions").Methods.FirstOrDefault(mtd => mtd.Name == "Constants");
                txt.strer = CecilHelper.Inject(mod, txt.strer);
                modType.Methods.Add(txt.strer);
                byte[] n = new byte[0x10]; rand.NextBytes(n);
                txt.strer.Name = Encoding.UTF8.GetString(n);
                txt.strer.IsAssembly = true;
                AddHelper(txt.strer, HelperAttribute.NoInjection);
                if (txt.isNative)
                {
                    txt.nativeDecr = new MethodDefinition(
                        ObfuscationHelper.GetNewName("NativeDecrypter" + Guid.NewGuid().ToString()),
                        MethodAttributes.Abstract | MethodAttributes.CompilerControlled |
                        MethodAttributes.ReuseSlot | MethodAttributes.Static,
                        mod.TypeSystem.Int32);
                    txt.nativeDecr.ImplAttributes = MethodImplAttributes.Native;
                    txt.nativeDecr.Parameters.Add(new ParameterDefinition(mod.TypeSystem.Int32));
                    modType.Methods.Add(txt.nativeDecr);
                }

                txt.key0 = rand.Next();
                txt.key1 = rand.Next();
                txt.key2 = rand.Next();
                txt.key3 = rand.Next();

                rand.NextBytes(n);
                byte[] dat = new byte[0x10];
                rand.NextBytes(dat);
                rand.NextBytes(txt.types);
                while (txt.types.Distinct().Count() != 5) rand.NextBytes(txt.types);

                txt.strer.Body.SimplifyMacros();
                foreach (Instruction inst in txt.strer.Body.Instructions)
                {
                    if ((inst.Operand as string) == "PADDINGPADDINGPADDING")
                        inst.Operand = Encoding.UTF8.GetString(n);
                    if ((inst.Operand as string) == "PADDINGPADDINGPADDINGPADDING")
                        inst.Operand = Encoding.UTF8.GetString(dat);
                    else if (inst.Operand is int && (int)inst.Operand == 12345678)
                        inst.Operand = txt.key0;
                    else if (inst.Operand is int && (int)inst.Operand == 0x67452301)
                        inst.Operand = txt.key1;
                    else if (inst.Operand is int && (int)inst.Operand == 0x3bd523a0)
                        inst.Operand = txt.key2;
                    else if (inst.Operand is int && (int)inst.Operand == 0x5f6f36c0)
                        inst.Operand = txt.key3;
                    else if (inst.Operand is int && (int)inst.Operand == 11)
                        inst.Operand = (int)txt.types[0];
                    else if (inst.Operand is int && (int)inst.Operand == 22)
                        inst.Operand = (int)txt.types[1];
                    else if (inst.Operand is int && (int)inst.Operand == 33)
                        inst.Operand = (int)txt.types[2];
                    else if (inst.Operand is int && (int)inst.Operand == 44)
                        inst.Operand = (int)txt.types[3];
                    else if (inst.Operand is int && (int)inst.Operand == 55)
                        inst.Operand = (int)txt.types[4];
                }

                txt.resId = Encoding.UTF8.GetString(n);
            }
            private void ProcessSafe(ConfusionParameter parameter)
            {
                _Context txt = cc.txts[mod];

                Random rand = new Random();
                TypeDefinition modType = mod.GetType("<Module>");

                AssemblyDefinition i = AssemblyDefinition.ReadAssembly(typeof(Iid).Assembly.Location);
                txt.strer = i.MainModule.GetType("Encryptions").Methods.FirstOrDefault(mtd => mtd.Name == "SafeConstants");
                txt.strer = CecilHelper.Inject(mod, txt.strer);
                modType.Methods.Add(txt.strer);
                byte[] n = new byte[0x10]; rand.NextBytes(n);
                txt.strer.Name = Encoding.UTF8.GetString(n);
                txt.strer.IsAssembly = true;

                txt.key0 = rand.Next();
                txt.key1 = rand.Next();
                txt.key2 = rand.Next();
                txt.key3 = rand.Next();

                rand.NextBytes(n);
                byte[] dat = new byte[0x10];
                rand.NextBytes(dat);
                rand.NextBytes(txt.types);
                while (txt.types.Distinct().Count() != 5) rand.NextBytes(txt.types);

                txt.strer.Body.SimplifyMacros();
                foreach (Instruction inst in txt.strer.Body.Instructions)
                {
                    if ((inst.Operand as string) == "PADDINGPADDINGPADDING")
                        inst.Operand = Encoding.UTF8.GetString(n);
                    if ((inst.Operand as string) == "PADDINGPADDINGPADDINGPADDING")
                        inst.Operand = Encoding.UTF8.GetString(dat);
                    else if (inst.Operand is int && (int)inst.Operand == 12345678)
                        inst.Operand = txt.key0;
                    else if (inst.Operand is int && (int)inst.Operand == 0x67452301)
                        inst.Operand = txt.key1;
                    else if (inst.Operand is int && (int)inst.Operand == 0x3bd523a0)
                        inst.Operand = txt.key2;
                    else if (inst.Operand is int && (int)inst.Operand == 0x5f6f36c0)
                        inst.Operand = txt.key3;
                    else if (inst.Operand is int && (int)inst.Operand == 11)
                        inst.Operand = (int)txt.types[0];
                    else if (inst.Operand is int && (int)inst.Operand == 22)
                        inst.Operand = (int)txt.types[1];
                    else if (inst.Operand is int && (int)inst.Operand == 33)
                        inst.Operand = (int)txt.types[2];
                    else if (inst.Operand is int && (int)inst.Operand == 44)
                        inst.Operand = (int)txt.types[3];
                    else if (inst.Operand is int && (int)inst.Operand == 55)
                        inst.Operand = (int)txt.types[4];
                }
                txt.strer.Body.OptimizeMacros();
                txt.strer.Body.ComputeOffsets();

                txt.resId = Encoding.UTF8.GetString(n);
            }
        }
        class Phase3 : StructurePhase, IProgressProvider
        {
            public Phase3(ConstantConfusion cc) { this.cc = cc; }
            ConstantConfusion cc;
            public override IConfusion Confusion
            {
                get { return cc; }
            }

            public override int PhaseID
            {
                get { return 3; }
            }

            public override Priority Priority
            {
                get { return Priority.Safe; }
            }

            public override bool WholeRun
            {
                get { return false; }
            }

            public override void Initialize(ModuleDefinition mod)
            {
                this.mod = mod;
            }

            public override void DeInitialize()
            {
                _Context txt = cc.txts[mod];
                MemoryStream str = new MemoryStream();
                using (BinaryWriter wtr = new BinaryWriter(new DeflateStream(str, CompressionMode.Compress)))
                {
                    foreach (Data dat in txt.dats)
                    {
                        wtr.Write(dat.Type);
                        wtr.Write(dat.Dat.Length);
                        wtr.Write(dat.Dat);
                    }
                }
                mod.Resources.Add(new EmbeddedResource(txt.resId, ManifestResourceAttributes.Private, str.ToArray()));
            }

            struct Context { public MethodDefinition mtd; public ILProcessor psr; public Instruction str;}
            ModuleDefinition mod;
            bool IsNull(object obj)
            {
                if (obj is int)
                    return (int)obj == 0;
                else if (obj is long)
                    return (long)obj == 0;
                else if (obj is float)
                    return (float)obj == 0;
                else if (obj is double)
                    return (double)obj == 0;
                else if (obj is string)
                    return string.IsNullOrEmpty((string)obj);
                else
                    return true;
            }
            void ExtractData(IList<Tuple<IAnnotationProvider, NameValueCollection>> mtds, List<Context> txts, bool num)
            {
                foreach (var tuple in mtds)
                {
                    MethodDefinition mtd = tuple.Item1 as MethodDefinition;
                    if (mtd == cc.txts[mod].strer || !mtd.HasBody) continue;
                    var bdy = mtd.Body;
                    bdy.SimplifyMacros();
                    var insts = bdy.Instructions;
                    ILProcessor psr = bdy.GetILProcessor();
                    bool hasDat = false;
                    for (int i = 0; i < insts.Count; i++)
                    {
                        if (insts[i].OpCode.Code == Code.Ldstr ||
                            (num && (insts[i].OpCode.Code == Code.Ldc_I4 ||
                            insts[i].OpCode.Code == Code.Ldc_I8 ||
                            insts[i].OpCode.Code == Code.Ldc_R4 ||
                            insts[i].OpCode.Code == Code.Ldc_R8)))
                        {
                            hasDat = true;
                            txts.Add(new Context() { mtd = mtd, psr = psr, str = insts[i] });
                        }
                    }
                    if (!hasDat) bdy.OptimizeMacros();
                }
            }
            byte[] GetOperand(object operand, out byte type)
            {
                byte[] ret;
                if (operand is double)
                {
                    ret = BitConverter.GetBytes((double)operand);
                    type = cc.txts[mod].types[0];
                }
                else if (operand is float)
                {
                    ret = BitConverter.GetBytes((float)operand);
                    type = cc.txts[mod].types[1];
                }
                else if (operand is int)
                {
                    ret = BitConverter.GetBytes((int)operand);
                    type = cc.txts[mod].types[2];
                }
                else if (operand is long)
                {
                    ret = BitConverter.GetBytes((long)operand);
                    type = cc.txts[mod].types[3];
                }
                else
                {
                    ret = Encoding.UTF8.GetBytes((string)operand);
                    type = cc.txts[mod].types[4];
                }
                return ret;
            }
            bool IsEqual(byte[] a, byte[] b)
            {
                int l = Math.Min(a.Length, b.Length);
                for (int i = 0; i < l; i++)
                    if (a[i] != b[i]) return false;
                return true;
            }
            void FinalizeBodies(List<Context> txts, int[] ids)
            {
                double total = txts.Count;
                int interval = 1;
                if (total > 1000)
                    interval = (int)total / 100;

                for (int i = 0; i < txts.Count; i++)
                {
                    int idx = txts[i].mtd.Body.Instructions.IndexOf(txts[i].str);
                    Instruction now = txts[i].str;
                    if (IsNull(now.Operand)) continue;
                    Instruction call = Instruction.Create(OpCodes.Call, cc.txts[mod].strer);
                    txts[i].psr.InsertAfter(idx, call);
                    if (now.Operand is int)
                        txts[i].psr.InsertAfter(call, Instruction.Create(OpCodes.Unbox_Any, txts[i].mtd.Module.TypeSystem.Int32));
                    else if (now.Operand is long)
                        txts[i].psr.InsertAfter(call, Instruction.Create(OpCodes.Unbox_Any, txts[i].mtd.Module.TypeSystem.Int64));
                    else if (now.Operand is float)
                        txts[i].psr.InsertAfter(call, Instruction.Create(OpCodes.Unbox_Any, txts[i].mtd.Module.TypeSystem.Single));
                    else if (now.Operand is double)
                        txts[i].psr.InsertAfter(call, Instruction.Create(OpCodes.Unbox_Any, txts[i].mtd.Module.TypeSystem.Double));
                    else
                        txts[i].psr.InsertAfter(call, Instruction.Create(OpCodes.Castclass, txts[i].mtd.Module.TypeSystem.String));
                    txts[i].psr.Replace(idx, Instruction.Create(OpCodes.Ldc_I4, ids[i]));
                    if (i % interval == 0 || i == txts.Count - 1)
                        progresser.SetProgress(i + 1, txts.Count);
                }

                List<int> hashs = new List<int>();
                for (int i = 0; i < txts.Count; i++)
                {
                    if (hashs.IndexOf(txts[i].mtd.GetHashCode()) == -1)
                    {
                        txts[i].mtd.Body.OptimizeMacros();
                        txts[i].mtd.Body.ComputeHeader();
                        hashs.Add(txts[i].mtd.GetHashCode());
                    }
                }
            }

            public override void Process(ConfusionParameter parameter)
            {
                if (parameter.GlobalParameters["type"] != "dynamic" &&
                    parameter.GlobalParameters["type"] != "native")
                {
                    ProcessSafe(parameter); return;
                }
                _Context txt = cc.txts[mod];

                List<Context> txts = new List<Context>();
                ExtractData(parameter.Target as IList<Tuple<IAnnotationProvider, NameValueCollection>>, txts, Array.IndexOf(parameter.GlobalParameters.AllKeys, "numeric") != -1);

                int[] ids = new int[txts.Count];
                txt.dict.Clear();
                var expGen = new ExpressionGenerator();
                int seed = expGen.Seed;
                if (txt.isNative)
                {
                    do
                    {
                        txt.exp = new ExpressionGenerator().Generate(6);
                        txt.invExp = ExpressionInverser.InverseExpression(txt.exp);
                    } while ((txt.visitor = new x86Visitor(txt.invExp, null)).RegisterOverflowed);
                }
                else
                {
                    txt.exp = expGen.Generate(10);
                    txt.invExp = ExpressionInverser.InverseExpression(txt.exp);
                }

                for (int i = 0; i < txts.Count; i++)
                {
                    object val = txts[i].str.Operand as object;
                    if (IsNull(val)) continue;

                    if (txt.dict.ContainsKey(val))
                        ids[i] = (int)(txt.dict[val] ^ ComputeHash(txts[i].mtd.MetadataToken.ToUInt32(), (uint)txt.key0, (uint)txt.key1, (uint)txt.key2, (uint)txt.key3));
                    else
                    {
                        ids[i] = (int)(txt.idx ^ ComputeHash(txts[i].mtd.MetadataToken.ToUInt32(), (uint)txt.key0, (uint)txt.key1, (uint)txt.key2, (uint)txt.key3));
                        byte t;
                        byte[] ori = GetOperand(val, out t);

                        int len;
                        byte[] dat = Encrypt(ori, txt.exp, out len);
                        byte[] final = new byte[dat.Length + 4];
                        Buffer.BlockCopy(dat, 0, final, 4, dat.Length);
                        Buffer.BlockCopy(BitConverter.GetBytes(len), 0, final, 0, 4);
                        txt.dats.Add(new Data() { Dat = final, Type = t });
                        txt.dict[val] = txt.idx;
                        txt.idx += final.Length + 5;
                    }
                }


                Instruction placeholder = null;
                foreach (Instruction inst in txt.strer.Body.Instructions)
                    if (inst.Operand is MethodReference && (inst.Operand as MethodReference).Name == "PlaceHolder")
                    {
                        placeholder = inst;
                        break;
                    }
                if (txt.isNative)
                    CecilHelper.Replace(txt.strer.Body, placeholder, new Instruction[]
                        {
                            Instruction.Create(OpCodes.Call, txt.nativeDecr)
                        });
                else
                {
                    Instruction ldloc = placeholder.Previous;
                    txt.strer.Body.Instructions.Remove(placeholder.Previous);   //ldloc
                    CecilHelper.Replace(txt.strer.Body, placeholder, new CecilVisitor(txt.invExp, new Instruction[]
                    {
                        ldloc
                    }).GetInstructions());
                }
                txt.strer.Body.OptimizeMacros();
                txt.strer.Body.ComputeOffsets();

                FinalizeBodies(txts, ids);
            }
            void ProcessSafe(ConfusionParameter parameter)
            {
                _Context txt = cc.txts[mod];

                List<Context> txts = new List<Context>();
                ExtractData(parameter.Target as IList<Tuple<IAnnotationProvider, NameValueCollection>>, txts, Array.IndexOf(parameter.GlobalParameters.AllKeys, "numeric") != -1);

                int[] ids = new int[txts.Count];
                for (int i = 0; i < txts.Count; i++)
                {
                    int idx = txts[i].mtd.Body.Instructions.IndexOf(txts[i].str);
                    object val = txts[i].str.Operand;
                    if (IsNull(val)) continue;

                    if (txt.dict.ContainsKey(val))
                        ids[i] = (int)(txt.dict[val] ^ ComputeHash(txts[i].mtd.MetadataToken.ToUInt32(), (uint)txt.key0, (uint)txt.key1, (uint)txt.key2, (uint)txt.key3));
                    else
                    {
                        byte t;
                        byte[] ori = GetOperand(val, out t);
                        byte[] dat = EncryptSafe(ori, txt.key0 ^ txt.idx);
                        ids[i] = (int)(txt.idx ^ ComputeHash(txts[i].mtd.MetadataToken.ToUInt32(), (uint)txt.key0, (uint)txt.key1, (uint)txt.key2, (uint)txt.key3));

                        txt.dats.Add(new Data() { Dat = dat, Type = t });
                        txt.dict[val] = txt.idx;
                        txt.idx += dat.Length + 5;
                    }
                }

                FinalizeBodies(txts, ids);
            }

            IProgresser progresser;
            public void SetProgresser(IProgresser progresser)
            {
                this.progresser = progresser;
            }
        }
        class MdPhase1 : MetadataPhase
        {
            public MdPhase1(ConstantConfusion cc) { this.cc = cc; }
            ConstantConfusion cc;
            public override IConfusion Confusion
            {
                get { return cc; }
            }

            public override int PhaseID
            {
                get { return 1; }
            }

            public override Priority Priority
            {
                get { return Priority.TypeLevel; }
            }

            public override void Process(NameValueCollection parameters, MetadataProcessor.MetadataAccessor accessor)
            {
                _Context txt = cc.txts[accessor.Module];
                if (!txt.isNative) return;

                txt.nativeRange = new Range(accessor.Codebase + (uint)accessor.Codes.Position, 0);
                MemoryStream ms = new MemoryStream();
                using (BinaryWriter wtr = new BinaryWriter(ms))
                {
                    wtr.Write(new byte[] { 0x8b, 0x44, 0x24, 0x04 });   //mov eax, [esp + 4]
                    wtr.Write(new byte[] { 0x53 });   //push ebx
                    wtr.Write(new byte[] { 0x50 });   //push eax
                    x86Register ret;
                    var insts = txt.visitor.GetInstructions(out ret);
                    foreach (var i in insts)
                        wtr.Write(i.Assemble());
                    if (ret != x86Register.EAX)
                        wtr.Write(
                            new x86Instruction()
                            {
                                OpCode = x86OpCode.MOV,
                                Operands = new Ix86Operand[]
                                {
                                    new x86RegisterOperand() { Register = x86Register.EAX },
                                    new x86RegisterOperand() { Register = ret }
                                }
                            }.Assemble());
                    wtr.Write(new byte[] { 0x5b });   //pop ebx
                    wtr.Write(new byte[] { 0xc3 });   //ret
                    wtr.Write(new byte[((ms.Length + 3) & ~3) - ms.Length]);
                }
                byte[] codes = ms.ToArray();
                accessor.Codes.WriteBytes(codes);
                accessor.SetCodePosition(accessor.Codebase + (uint)accessor.Codes.Position);
                txt.nativeRange.Length = (uint)codes.Length;
            }
        }
        class MdPhase2 : MetadataPhase
        {
            public MdPhase2(ConstantConfusion cc) { this.cc = cc; }
            ConstantConfusion cc;
            public override IConfusion Confusion
            {
                get { return cc; }
            }

            public override int PhaseID
            {
                get { return 2; }
            }

            public override Priority Priority
            {
                get { return Priority.TypeLevel; }
            }

            public override void Process(NameValueCollection parameters, MetadataProcessor.MetadataAccessor accessor)
            {
                _Context txt = cc.txts[accessor.Module];
                if (!txt.isNative) return;

                var tbl = accessor.TableHeap.GetTable<MethodTable>(Table.Method);
                var row = tbl[(int)txt.nativeDecr.MetadataToken.RID - 1];
                row.Col2 = MethodImplAttributes.Native | MethodImplAttributes.Unmanaged | MethodImplAttributes.PreserveSig;
                row.Col3 &= ~MethodAttributes.Abstract;
                row.Col3 |= MethodAttributes.PInvokeImpl;
                row.Col1 = txt.nativeRange.Start;
                accessor.BodyRanges[txt.nativeDecr.MetadataToken] = txt.nativeRange;

                tbl[(int)txt.nativeDecr.MetadataToken.RID - 1] = row;

                accessor.Module.Attributes &= ~ModuleAttributes.ILOnly;
            }
        }


        struct Data
        {
            public byte[] Dat;
            public byte Type;
        }
        class _Context
        {
            public List<Data> dats;
            public Dictionary<object, int> dict;
            public int idx = 0;

            public string resId;
            public int key0;
            public int key1;
            public int key2;
            public int key3;
            public byte[] types = new byte[5];
            public MethodDefinition strer;

            public bool isNative;
            public Range nativeRange;
            public MethodDefinition nativeDecr;
            public Expression exp;
            public Expression invExp;
            public x86Visitor visitor;
        }
        Dictionary<ModuleDefinition, _Context> txts = new Dictionary<ModuleDefinition, _Context>();

        public string ID
        {
            get { return "const encrypt"; }
        }
        public string Name
        {
            get { return "Constants Confusion"; }
        }
        public string Description
        {
            get { return "This confusion obfuscate the constants in the code and store them in a encrypted and compressed form."; }
        }
        public Target Target
        {
            get { return Target.Methods; }
        }
        public Preset Preset
        {
            get { return Preset.Minimum; }
        }
        public bool StandardCompatible
        {
            get { return true; }
        }
        public bool SupportLateAddition
        {
            get { return true; }
        }
        public Behaviour Behaviour
        {
            get { return Behaviour.Inject | Behaviour.AlterCode | Behaviour.Encrypt; }
        }

        Phase[] ps;
        public Phase[] Phases
        {
            get
            {
                if (ps == null)
                    ps = new Phase[] { new Phase1(this), new Phase3(this), new MdPhase1(this), new MdPhase2(this) };
                return ps;
            }
        }

        public void Init() { txts.Clear(); }
        public void Deinit() { txts.Clear(); }

        static void Write7BitEncodedInt(BinaryWriter wtr, int value)
        {
            // Write out an int 7 bits at a time. The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value; // support negative numbers
            while (v >= 0x80)
            {
                wtr.Write((byte)(v | 0x80));
                v >>= 7;
            }
            wtr.Write((byte)v);
        }
        static int Read7BitEncodedInt(BinaryReader rdr)
        {
            // Read out an int 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                b = rdr.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        private static byte[] Encrypt(byte[] bytes, Expression exp, out int len)
        {
            byte[] tmp = new byte[(bytes.Length + 7) & ~7];
            Buffer.BlockCopy(bytes, 0, tmp, 0, bytes.Length);
            len = bytes.Length;

            MemoryStream ret = new MemoryStream();
            using (BinaryWriter wtr = new BinaryWriter(ret))
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    int en = (int)ExpressionEvaluator.Evaluate(exp, tmp[i]);
                    Write7BitEncodedInt(wtr, en);
                }
            }

            return ret.ToArray();
        }
        private static byte[] Decrypt(byte[] bytes, int len, Expression exp)
        {
            byte[] ret = new byte[(len + 7) & ~7];

            using (BinaryReader rdr = new BinaryReader(new MemoryStream(bytes)))
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    int r = Read7BitEncodedInt(rdr);
                    ret[i] = (byte)ExpressionEvaluator.Evaluate(exp, r);
                }
            }

            return ret;
        }
        private static byte[] EncryptSafe(byte[] bytes, int key)
        {
            ushort _m = (ushort)(key >> 16);
            ushort _c = (ushort)(key & 0xffff);
            ushort m = _c; ushort c = _m;
            byte[] ret = (byte[])bytes.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] ^= (byte)((key * m + c) % 0x100);
                m = (ushort)((key * m + _m) % 0x10000);
                c = (ushort)((key * c + _c) % 0x10000);
            }
            return ret;
        }


        static uint ComputeHash(uint x, uint key, uint init0, uint init1, uint init2)
        {
            uint h = init0 ^ x;
            uint h1 = init1;
            uint h2 = init2;
            for (uint i = 1; i <= 64; i++)
            {
                h = (h & 0x00ffffff) << 8 | ((h & 0xff000000) >> 24);
                uint n = (h & 0xff) % 64;
                if (n >= 0 && n < 16)
                {
                    h1 |= (((h & 0x0000ff00) >> 8) & ((h & 0x00ff0000) >> 16)) ^ (~h & 0x000000ff);
                    h2 ^= (h * i + 1) % 16;
                    h += (h1 | h2) ^ key;
                }
                else if (n >= 16 && n < 32)
                {
                    h1 ^= ((h & 0x00ff00ff) << 8) ^ (((h & 0x00ffff00) >> 8) | (~h & 0x0000ffff));
                    h2 += (h * i) % 32;
                    h |= (h1 + ~h2) & key;
                }
                else if (n >= 32 && n < 48)
                {
                    h1 += ((h & 0x000000ff) | ((h & 0x00ff0000) >> 16)) + (~h & 0x000000ff);
                    h2 -= ~(h + n) % 48;
                    h ^= (h1 % h2) | key;
                }
                else if (n >= 48 && n < 64)
                {
                    h1 ^= (((h & 0x00ff0000) >> 16) | ~(h & 0x0000ff)) * (~h & 0x00ff0000);
                    h2 += (h ^ i - 1) % n;
                    h -= ~(h1 ^ h2) + key;
                }
            }
            return h;
        }
    }
}
