#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class SubHandler : ITranslationHandler
	{
		public Code ILCode => Code.Sub;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 2);
			IRVariable ret = tr.Context.AllocateVRegister(expr.Type.Value);
			if (expr.Type.HasValue && (expr.Type.Value == ASTType.R4 || expr.Type.Value == ASTType.R8))
			{
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
				{
					Operand1 = ret,
					Operand2 = tr.Translate(expr.Arguments[0])
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.SUB)
				{
					Operand1 = ret,
					Operand2 = tr.Translate(expr.Arguments[1])
				});
			}
			else
			{
				IRVariable tmp = tr.Context.AllocateVRegister(expr.Type.Value);
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
				{
					Operand1 = ret,
					Operand2 = tr.Translate(expr.Arguments[0])
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
				{
					Operand1 = tmp,
					Operand2 = tr.Translate(expr.Arguments[1])
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.__NOT)
				{
					Operand1 = tmp
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.ADD)
				{
					Operand1 = ret,
					Operand2 = tmp
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.ADD)
				{
					Operand1 = ret,
					Operand2 = IRConstant.FromI4(1)
				});
			}
			return ret;
		}
	}
}