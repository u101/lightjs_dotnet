namespace LightJS.Outsource;

public class Mather
{
    //	Хранит инфиксное выражение
    public string infixExpr { get; private set; }

    //	Хранит постфиксное выражение
    public string postfixExpr { get; private set; }

    //	Список и приоритет операторов
    private Dictionary<char, int> operationPriority = new()
    {
        { '(', 0 },
        { '+', 1 },
        { '-', 1 },
        { '*', 2 },
        { '/', 2 },
        { '^', 3 },
        { '~', 4 } //	Унарный минус
    };

    //	Конструктор класса
    public Mather(string expression)
    {
        //	Инициализируем поля
        infixExpr = expression;
        postfixExpr = ToPostfix(infixExpr + "\r");
    }

    private string ToPostfix(string infixExpr)
    {
        //	Выходная строка, содержащая постфиксную запись
        string postfixExpr = "";
        //	Инициализация стека, содержащий операторы в виде символов
        Stack<char> stack = new();

        //	Перебираем строку
        for (int i = 0; i < infixExpr.Length; i++)
        {
            //	Текущий символ
            char c = infixExpr[i];

            //	Если симовол - цифра
            if (Char.IsDigit(c))
            {
                //	Парсии его, передав строку и текущую позицию, и заносим в выходную строку
                postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
            }
            //	Если открывающаяся скобка 
            else if (c == '(')
            {
                //	Заносим её в стек
                stack.Push(c);
            }
            //	Если закрывающая скобка
            else if (c == ')')
            {
                //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                while (stack.Count > 0 && stack.Peek() != '(')
                    postfixExpr += stack.Pop();
                //	Удаляем открывающуюся скобку из стека
                stack.Pop();
            }
            //	Проверяем, содержится ли символ в списке операторов
            else if (operationPriority.ContainsKey(c))
            {
                //	Если да, то сначала проверяем
                char op = c;
                //	Является ли оператор унарным символом
                if (op == '-' && (i == 0 || (i > 1 && operationPriority.ContainsKey(infixExpr[i - 1]))))
                    //	Если да - преобразуем его в тильду
                    op = '~';

                //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                while (stack.Count > 0 && (operationPriority[stack.Peek()] >= operationPriority[op]))
                    postfixExpr += stack.Pop();
                //	Заносим в стек оператор
                stack.Push(op);
            }
        }

        //	Заносим все оставшиеся операторы из стека в выходную строку
        foreach (char op in stack)
            postfixExpr += op;

        //	Возвращаем выражение в постфиксной записи
        return postfixExpr;
    }

    /// <summary>
    /// Парсинг целочисленных значений
    /// </summary>
    /// <param name="expr">Строка для парсинга</param>
    /// <param name="pos">Позиция</param>
    /// <returns>Число в виде строки</returns>
    private string GetStringNumber(string expr, ref int pos)
    {
        //	Хранит число
        string strNumber = "";

        //	Перебираем строку
        for (; pos < expr.Length; pos++)
        {
            //	Разбираемый символ строки
            char num = expr[pos];

            //	Проверяем, является символ числом
            if (Char.IsDigit(num))
                //	Если да - прибавляем к строке
                strNumber += num;
            else
            {
                //	Если нет, то перемещаем счётчик к предыдущему символу
                pos--;
                //	И выходим из цикла
                break;
            }
        }

        //	Возвращаем число
        return strNumber;
    }

    /// <summary>
    /// Вычисляет значения, согласно оператору
    /// </summary>
    /// <param name="op">Оператор</param>
    /// <param name="first">Первый операнд (перед оператором)</param>
    /// <param name="second">Второй операнд (после оператора)</param>
    private double Execute(char op, double first, double second) => op switch
    {
        '+' => first + second, //	Сложение
        '-' => first - second, //	Вычитание
        '*' => first * second, //	Умножение
        '/' => first / second, //	Деление
        '^' => Math.Pow(first, second), //	Степень
        _ => 0 //	Возвращает, если не был найден подходящий оператор
    };

    public double Calc()
    {
        //	Стек для хранения чисел
        Stack<double> locals = new();
        //	Счётчик действий
        int counter = 0;

        //	Проходим по строке
        for (int i = 0; i < postfixExpr.Length; i++)
        {
            //	Текущий символ
            char c = postfixExpr[i];

            //	Если символ число
            if (Char.IsDigit(c))
            {
                //	Парсим
                string number = GetStringNumber(postfixExpr, ref i);
                //	Заносим в стек, преобразовав из String в Double-тип
                locals.Push(Convert.ToDouble(number));
            }
            //	Если символ есть в списке операторов
            else if (operationPriority.ContainsKey(c))
            {
                //	Прибавляем значение счётчику
                counter += 1;
                //	Проверяем, является ли данный оператор унарным
                if (c == '~')
                {
                    //	Проверяем, пуст ли стек: если да - задаём нулевое значение,
                    //	еси нет - выталкиваем из стека значение
                    double last = locals.Count > 0 ? locals.Pop() : 0;

                    //	Получаем результат операции и заносим в стек
                    locals.Push(Execute('-', 0, last));
                    //	Отчитываемся пользователю о проделанной работе
                    Console.WriteLine($"{counter}) {c}{last} = {locals.Peek()}");
                    //	Указываем, что нужно перейти к следующей итерации цикла
                    //	 для того, чтобы пропустить остальной код
                    continue;
                }

                //	Получаем значения из стека в обратном порядке
                double second = locals.Count > 0 ? locals.Pop() : 0,
                    first = locals.Count > 0 ? locals.Pop() : 0;

                //	Получаем результат операции и заносим в стек
                locals.Push(Execute(c, first, second));
                //	Отчитываемся пользователю о проделанной работе
                Console.WriteLine($"{counter}) {first} {c} {second} = {locals.Peek()}");
            }
        }

        //	По завершению цикла возвращаем результат из стека
        return locals.Pop();
    }
}