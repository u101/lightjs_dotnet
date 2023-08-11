namespace LightJS.Outsource;

public static class MatherAdv
{

    private static readonly Dictionary<MatherTokenType, int> _opsPriorityMap = new()
    {
        
        { MatherTokenType.OpAssign , 25},
        { MatherTokenType.OpParenthesesOpen , 50},
        
        { MatherTokenType.OpPlus , 100},
        { MatherTokenType.OpMinus , 100},
        { MatherTokenType.OpMul , 200},
        { MatherTokenType.OpDiv , 200},
    };

    private class Op : IMatherNode
    {
        public MatherTokenType TokenType { get; }

        public Op(MatherTokenType tokenType)
        {
            TokenType = tokenType;
        }
    }
    

    public static IMatherNode Convert(List<MatherToken> tokens)
    {
        //	Выходная строка, содержащая постфиксную запись
        var postfixExpr = new List<IMatherNode>();
        //	Инициализация стека, содержащий операторы в виде символов
        var stack = new Stack<Op>();

        //	Перебираем строку
        for (var i = 0; i < tokens.Count; i++)
        {
            //	Текущий символ
            var c = tokens[i];

            //	Если симовол - цифра
            if (c.TokenType == MatherTokenType.Id)
            {
                postfixExpr.Add(new MatherGetVarNode(c.Value));
            }
            else if (c.TokenType == MatherTokenType.Literal)
            {
                postfixExpr.Add(new MatherLiteralNode(c.Value));
            }
            
            //	Если открывающаяся скобка 
            else if (c.TokenType == MatherTokenType.OpParenthesesOpen)
            {
                //	Заносим её в стек
                stack.Push(new Op(c.TokenType));
            }
            //	Если закрывающая скобка
            else if (c.TokenType == MatherTokenType.OpParenthesesClose)
            {
                //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                while (stack.Count > 0 && stack.Peek().TokenType != MatherTokenType.OpParenthesesOpen)
                    postfixExpr.Add(stack.Pop()); 
                //	Удаляем открывающуюся скобку из стека
                stack.Pop();
            }
            //	Проверяем, содержится ли символ в списке операторов
            else if (_opsPriorityMap.ContainsKey(c.TokenType))
            {
                //	Если да, то сначала проверяем
                // char op = c;
                //	Является ли оператор унарным символом
                // if (op == '-' && (i == 0 || (i > 1 && operationPriority.ContainsKey(infixExpr[i - 1]))))
                //     //	Если да - преобразуем его в тильду
                //     op = '~';

                //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                while (stack.Count > 0 && (_opsPriorityMap[stack.Peek().TokenType] >= _opsPriorityMap[c.TokenType]))
                    postfixExpr.Add(stack.Pop());
                
                //	Заносим в стек оператор
                stack.Push(new Op(c.TokenType));
            }
        }

        //	Заносим все оставшиеся операторы из стека в выходную строку
        while (stack.Count > 0)
        {
            postfixExpr.Add(stack.Pop());
        }
        
        // --------- CREATE NODES
        
        //	Стек для хранения чисел
        var locals = new Stack<IMatherNode>();
        //	Счётчик действий
        var counter = 0;

        //	Проходим по строке
        for (var i = 0; i < postfixExpr.Count; i++)
        {
            //	Текущий символ
            var c = postfixExpr[i];
            
            //	Если символ есть в списке операторов
            if (c is Op op)
            {
                //	Прибавляем значение счётчику
                counter += 1;
                //	Проверяем, является ли данный оператор унарным
                // if (c == '~')
                // {
                //     //	Проверяем, пуст ли стек: если да - задаём нулевое значение,
                //     //	еси нет - выталкиваем из стека значение
                //     double last = locals.Count > 0 ? locals.Pop() : 0;
                //
                //     //	Получаем результат операции и заносим в стек
                //     locals.Push(Execute('-', 0, last));
                //     //	Отчитываемся пользователю о проделанной работе
                //     Console.WriteLine($"{counter}) {c}{last} = {locals.Peek()}");
                //     //	Указываем, что нужно перейти к следующей итерации цикла
                //     //	 для того, чтобы пропустить остальной код
                //     continue;
                // }

                //	Получаем значения из стека в обратном порядке
                var right = locals.Pop();
                var left = locals.Pop();

                //	Получаем результат операции и заносим в стек
                locals.Push(new MatherBinaryOpNode(left, right, op.TokenType));
                //	Отчитываемся пользователю о проделанной работе
                Console.WriteLine($"{counter}) {left} {c} {right} = {locals.Peek()}");
            }
            else
            {
                locals.Push(c);
            }
        }

        //	По завершению цикла возвращаем результат из стека
        return locals.Pop();

        
    }
    
}