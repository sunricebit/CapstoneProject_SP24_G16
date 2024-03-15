﻿namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = 64;
            int d = 2;


            int a = Convert.ToInt32(Math.Log2(n));
            int w = Convert.ToInt32(Math.Log2(d));
            int x = 0;

            for (int i = (a - 1); i >= w; i--)
            {
                x = x + Convert.ToInt32(Math.Pow(2, i));
            }

            int y = 2 * x;

            Console.WriteLine(x);
            //Console.WriteLine(y);

            for (int i = 1; i <= 123; i++)
            {
                int resultWin = win(i, n, d, x, y, w, a);
                int resultLose = lose(i, n, d, x, y, w, a);
                Console.WriteLine(i + "\tw:" + resultWin + "\tl:" + resultLose);
            }
        }

        public static int win(int m, int n, int d, int x, int y, int w, int a)
        {
            int from = 0;
            int to = 0;
            if (m % 2 == 1 && m <= x)
            {
                return (n / 2 + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m <= x)
            {
                return n / 2 + m / 2;
            }
            else if (m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return m * 2 + d / 2 - (m - x);
            }
            else if (m % 2 == 1 && m > (y + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return Convert.ToInt32(0.5 * y + 0.75 * d + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m > (y + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return Convert.ToInt32(0.5 * y + 0.75 * d + m / 2);
            }
            else if (m > y && m <= (y + d / 2))
            {
                if (m > (y + d / 4))
                {
                    return m + d / 4;
                }
                else
                {
                    return m + d - d / 4;
                }
            }
            else
            {
                from = x + d / 2 + 1;
                to = x + d / 2 + Convert.ToInt32(Math.Pow(2, a - 2));
                for (int i = (a - 2); i >= (w - 1); i--)
                {
                    if (m >= from && m <= to)
                    {
                        return m + Convert.ToInt32(Math.Pow(2, i));
                    }
                    else if (m > to && m <= (to + Convert.ToInt32(Math.Pow(2, i))))
                    {
                        if (m % 2 == 1)
                        {
                            return Convert.ToInt32((m + 1) / 2 + 0.5 * to + Math.Pow(2, i));
                        }
                        else if (m % 2 == 0)
                        {
                            return Convert.ToInt32(m / 2 + 0.5 * to + Math.Pow(2, i));
                        }
                    }
                    from += Convert.ToInt32(2 * Math.Pow(2, i));
                    to += Convert.ToInt32(1.5 * Math.Pow(2, i));
                }
            }
            return 0;
        }

        public static int lose(int m, int n, int d, int x, int y, int w, int a)
        {
            if (m % 2 == 1 && m <= n / 2)
            {
                return ((x + d / 2) + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m <= n / 2)
            {
                return (x + d / 2) + m / 2;
            }
            else if (m > n / 2 && m <= 0.75 * n)
            {
                return (x + d / 2 + n + (1 - m));
            }
            else if (m > 0.75 * n && m <= x)
            {
                for (int i = a - 3; i >= w; i--)
                {
                    int count1 = 0;
                    int count2 = 0;
                    for (int j = a - 1; j >= i + 1; j--)
                    {
                        count1 += Convert.ToInt32(Math.Pow(2, j));
                    }

                    count2 = count1 + Convert.ToInt32(Math.Pow(2, i));

                    if (m > count1 && m <= count2)
                    {
                        if (m % 2 == 1)
                        {
                            return x + d / 2 + count1 - (count2 - m) + 1;
                        }
                        else if (m % 2 == 0)
                        {
                            return x + d / 2 + count1 - (count2 - m) - 1;
                        }
                    }
                }

            }
            else if (m % 2 == 1 && m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return y + (m - x + 1);
            }
            else if (m % 2 == 0 && m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return y + (m - x - 1);
            }
            return 0;
        }
    }
}