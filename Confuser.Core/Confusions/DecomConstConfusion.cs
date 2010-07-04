﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Confuser.Core.Poly;
using Confuser.Core.Poly.Visitors;

namespace Confuser.Core.Confusions
{
    public class DecomConstConfusion : StructureConfusion
    {
        public override void PreConfuse(Confuser cr, AssemblyDefinition asm)
        {
            throw new InvalidOperationException();
        }

        public override void DoConfuse(Confuser cr, AssemblyDefinition asm)
        {
            throw new InvalidOperationException();
        }

        public override Priority Priority
        {
            get { return Priority.CodeLevel; }
        }

        public override string Name
        {
            get { return "Constant Decomposition Confusion"; }
        }

        public override ProcessType Process
        {
            get { return ProcessType.Post; }
        }

        public override bool StandardCompatible
        {
            get { return true; }
        }



        public override void PostConfuse(Confuser cr, AssemblyDefinition asm)
        {
            foreach (TypeDefinition t in asm.MainModule.GetAllTypes())
                ProcessMethods(cr, t);
        }

        private void ProcessMethods(Confuser cr, TypeDefinition def)
        {
            foreach (MethodDefinition mtd in def.Methods)
            {
                ProcessMethod(cr, mtd);
            }
        }
        private void ProcessMethod(Confuser cr, MethodDefinition mtd)
        {
            if (!mtd.HasBody) return;
            MethodBody body = mtd.Body;
            body.SimplifyMacros();
            ILProcessor psr=body.GetILProcessor();
            for (int i = 0; i < body.Instructions.Count; i++)
            {
                if (body.Instructions[i].OpCode.Name == "ldc.i4"||
                    body.Instructions[i].OpCode.Name == "ldc.i8"||
                    body.Instructions[i].OpCode.Name == "ldc.r4"||
                    body.Instructions[i].OpCode.Name == "ldc.r8")
                {
                    double val = Convert.ToDouble(body.Instructions[i].Operand);
                    int seed;

                    Expression exp;
                    double eval = 0;
                    double tmp = 0;
                    do
                    {
                        exp = ExpressionGenerator.Generate(4, out seed);
                        eval = (double)new ReflectionVisitor(exp, false, true).Eval(val);
                        try
                        {
                            tmp = (double)new ReflectionVisitor(exp, true, true).Eval(eval);
                        }
                        catch { continue; }
                    } while (tmp != val);

                    Instruction[] expInsts = new CecilVisitor(exp, true, new Instruction[] { Instruction.Create(OpCodes.Ldc_R8, eval) }, true).GetInstructions();
                    if (expInsts.Length == 0) continue;
                    string op = body.Instructions[i].OpCode.Name;
                    psr.Replace(body.Instructions[i], expInsts[0]);
                    for (int ii = 1; ii < expInsts.Length; ii++)
                    {
                        psr.InsertAfter(expInsts[ii - 1], expInsts[ii]);
                        i++;
                    }
                    switch (op)
                    {
                        case "ldc.i4":
                            psr.InsertAfter(expInsts[expInsts.Length - 1], Instruction.Create(OpCodes.Conv_I4)); break;
                        case "ldc.i8":
                            psr.InsertAfter(expInsts[expInsts.Length - 1], Instruction.Create(OpCodes.Conv_I8)); break;
                        case "ldc.r4":
                            psr.InsertAfter(expInsts[expInsts.Length - 1], Instruction.Create(OpCodes.Conv_R4)); break;
                        case "ldc.r8":
                            psr.InsertAfter(expInsts[expInsts.Length - 1], Instruction.Create(OpCodes.Conv_R8)); break;
                    }
                }
            }
        }
    }
}