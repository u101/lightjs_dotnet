using App16.LightJS.Program;

namespace App16.LightJS.Tests.Compiler;

public sealed class InstructionsBuilder
{

    private readonly List<Entry> _entries = new();

    private readonly Dictionary<string, int> _locals = new();

    public void DefineLocal(string name)
    {
        _locals[name] = _locals.Count;
    }

    public Entry Push(LjsInstructionCode instructionCode)
    {
        var e = new DirectInstructionEntry(new LjsInstruction(instructionCode));
        _entries.Add(e);
        return e;
    }
    
    public Entry Push(LjsInstructionCode instructionCode, int arg)
    {
        var e = new DirectInstructionEntry(new LjsInstruction(instructionCode, arg));
        _entries.Add(e);
        return e;
    }

    public Entry StoreLocal(string localName)
    {
        var localIndex = _locals[localName];
        var e = new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.VarStore, localIndex));
        _entries.Add(e);
        return e;
    }

    public Entry LoadLocal(string localName)
    {
        var localIndex = _locals[localName];
        var e = new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.VarLoad, localIndex));
        _entries.Add(e);
        return e;
    }

    public Entry JumpIfFalse(string label)
    {
        var e = new JumpInstruction(LjsInstructionCode.JumpIfFalse, label);
        _entries.Add(e);
        return e;
    }
    
    public Entry Jump(string label)
    {
        var e = new JumpInstruction(LjsInstructionCode.Jump, label);
        _entries.Add(e);
        return e;
    }

    public Entry PostIncrement(string localName)
    {
        var localIndex = _locals[localName];
        
        var e = new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.VarLoad, localIndex));
        _entries.Add(e);
        _entries.Add(new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.Copy)));
        _entries.Add(new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.ConstIntOne)));
        _entries.Add(new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.Add)));
        _entries.Add(new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.VarStore, localIndex)));
        _entries.Add(new DirectInstructionEntry(new LjsInstruction(LjsInstructionCode.Discard)));

        return e;
    }

    public LjsInstruction[] Build()
    {
        var result = new LjsInstruction[_entries.Count];

        for (var i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            switch (e)
            {
                case DirectInstructionEntry direct:
                    result[i] = direct.Instruction;
                    break;
                case JumpInstruction jumpInstruction:
                    result[i] = new LjsInstruction(jumpInstruction.InstructionCode,
                        GetInstructionIndexAtLabel(jumpInstruction.ToLabel));
                    break;
                default:
                    throw new Exception();
            }
        }

        return result;
    }

    private int GetInstructionIndexAtLabel(string label)
    {
        for (var i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (label == e.Label) return i;
        }

        throw new Exception($"label not found {label}");
    }


    public abstract class Entry
    {
        public string Label { get; set; } = string.Empty;
    }
    
    private class JumpInstruction : Entry
    {
        public LjsInstructionCode InstructionCode { get; }
        public string ToLabel { get; }

        public JumpInstruction(LjsInstructionCode instructionCode, string toLabel)
        {
            InstructionCode = instructionCode;
            ToLabel = toLabel;
        }
    }
    
    private class DirectInstructionEntry : Entry
    {
        public LjsInstruction Instruction { get; }

        public DirectInstructionEntry(LjsInstruction instruction)
        {
            Instruction = instruction;
        }
    }
    
}