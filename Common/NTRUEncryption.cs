using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NTRUEncryption
    {


        private void InitializeParameter(out int N, out int v, out int p, out int q, out int[] Fv, out int[] Fq, out int[] Fp, out int[] h, out int[] f, out int[] g, out int[] r)
        {
            N = 11;//维数N
            v = 2;//参数2
            p = 3;
            q = 1024;
            int[] poly = new int[N];
            int[] b = new int[N];
            Fv = new int[N];//存放求出的Fv=F2
            Fq = new int[N];//存放求出的Fq
            Fp = new int[N];//存放求出的Fp
            h = new int[N];//存放求出公钥h
            f = new int[] { 1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
            g = new int[] { -1, 0, 1, 1, 0, 1, 0, 0, -1, 0, -1 };
            r = new int[] { -1, 0, 1, 1, 1, -1, 0, -1, 0, 0, 0 };
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Encryption(string message)
        {

            int N;
            int v;
            int p;
            int q;
            int[] Fv;
            int[] Fq;
            int[] Fp;
            int[] h;
            int[] f;
            int[] g;
            int[] r;
            InitializeParameter(out N, out v, out p, out q, out Fv, out Fq, out Fp, out h, out f, out g, out r);
            string ciphertext = "";

            string sm = WordTo8Bit(message);
            int k = sm.Length / N;
            char[] cm = new char[sm.Length];
            cm = sm.ToCharArray();
            int[] mt = new int[sm.Length];

            for (int i = 0; i < sm.Length; i++)
            {
                mt[i] = Convert.ToInt32(cm[i]) - 48;

            }

            for (int t = 0; t < k; t++)
            {
                int[] m = new int[N];
                for (int i = 0; i < N; i++)
                {
                    m[i] = mt[i + t * N];
                }
                int[] fv = new int[] { 1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fv = InversePoly(fv, N, v);
                int[] m1 = new int[N];//存放解密后的明文，与之前的明文作对比
                int[] E = new int[N];//存放得到的密文
                int[] f1 = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fq = Inv(f1, Fv, N, v, q);
                int[] f2 = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fp = InversePoly(f2, N, p);
                //先生成公私钥
                int[] f3 = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                h = NTRUcreateKey(N, p, q, f3, g, h, Fp, Fq);
                //将公钥输出到窗口

                //加密
                E = Encode(N, q, r, m, h);

                for (int i = 0; i <= N - 1; i++)
                {
                    string E1 = E[i].ToString();
                    ciphertext += E1 + ",";
                }
            }

            #region////输入的明文不是N的整数倍

            if (sm.Length % N != 0)
            {
                int yu = sm.Length % N;
                int[] myu = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (int i = 0; i < yu; i++)
                {
                    myu[i] = mt[i + sm.Length / N * N];
                }
                int[] fvyu = new int[] { 1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fv = InversePoly(fvyu, N, v);
                int[] m1yu = new int[N];//存放解密后的明文，与之前的明文作对比
                int[] Eyu = new int[N];//存放得到的密文
                int[] f1yu = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fq = Inv(f1yu, Fv, N, v, q);
                int[] f2yu = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                Fp = InversePoly(f2yu, N, p);
                //先生成公私钥
                int[] f3yu = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                h = NTRUcreateKey(N, p, q, f3yu, g, h, Fp, Fq);
                //将公钥输出到窗口

                //this.labelPK.Text = OutputKey(h);
                //将私钥输出到窗口
                //this.labelSK.Text = "(" + OutputKey(f) + "," + "\n" + OutputKey(Fq) + ")";

                //加密
                // #########################################################################
                Eyu = Encode(N, q, r, myu, h);

                for (int i = 0; i < Eyu.Length; i++)
                {
                    string E1yu = Eyu[i].ToString();
                    ciphertext += E1yu + ",";
                }
                //// #########################################################################
                ////解密
                //// #########################################################################
                //int[] f4yu = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                //Fp = InversePoly(f4yu, N, p);
                //int[] ayu = new int[N];
                //int[] fayu = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                //ayu = MultiPloy(fayu, Eyu, N, q);
                //m1yu = Decode(N, p, q, ayu, Fp, Eyu);
                //for (int i = 0; i < yu; i++)
                //{
                //    string m2yu = m1yu[i].ToString();
                //    this.textBoxM1.Text += m2yu + ",";
                //}
            }
            #endregion
            return ciphertext;
        }

        public string Decryption(string ciphertext)
        {

            #region 参数初始化
            int N;
            int v;
            int p;
            int q;
            int[] Fv;
            int[] Fq;
            int[] Fp;
            int[] h;
            int[] f;
            int[] g;
            int[] r;
            //初始化参数
            InitializeParameter(out N, out v, out p, out q, out Fv, out Fq, out Fp, out h, out f, out g, out r);

            #endregion
            // #########################################################################
            //解密
            // #########################################################################
            int[] f4 = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
            Fp = InversePoly(f4, N, p);
            int[] a = new int[N];

            string[] EstringArray = ciphertext.Split(',');//将解密的密文写入字符串
            int[] EintArray = new int[EstringArray.Length];//将密文字符串转化到int 数组中
            for (int i = 0; i < EstringArray.Length - 1; i++)
            {
                EintArray[i] = int.Parse(EstringArray[i]);
            }
            int[] E = new int[N];
            int[] m1 = new int[N];
            int[] temp = new int[EstringArray.Length];

            //#region 解密过程中输入要解密的长度是N的倍数

            for (int count = 0; count < (EstringArray.Length - 1) / N; count++)
            {
                for (int j = 0; j < N; j++)
                {
                    E[j] = EintArray[j + count * N];
                }
                int[] fa = new int[] { -1, 1, 1, 0, -1, 0, 1, 0, 0, 1, -1 };
                a = MultiPloy(fa, E, N, q);

                m1 = Decode(N, p, q, a, Fp, E);//解密
                for (int i = 0; i <= N - 1; i++)
                {
                    temp[i + count * N] = m1[i];
                    //string m2 = m1[i].ToString();
                    //this.textBoxM1.Text += m2;// +",";
                }

            }
            //将解密的明文显示在窗口
            string str = null;
            for (int j = 0; j < (EintArray.Length / 8) * 8; j++)
            {
                str += temp[j];// +",";

            }
            return BitToWord(str);
        }
        public int[] Inv(int[] a, int[] b, int N, int q, int qr)
        {


            int v = q;
            while (v < qr)
            {
                v = v * 2;
                int[] c = new int[N];
                c = MultiPloy(a, b, N, v);

                c[0] = 2 - c[0];
                b = MultiPloy(b, c, N, v);
            }
            return b;

        }
        public string WordTo8Bit(string str)
        {
            ASCIIEncoding ae = new ASCIIEncoding();
            byte[] ByteArray = ae.GetBytes(str);
            string s = null;
            for (int i = 0; i < ByteArray.Length; i++)
            {
                s += Convert.ToString(ByteArray[i], 2).PadLeft(8, '0');
            }
            return s;

        }
        public string BitToWord(string bitString)
        {
            //将8位的二进制串转化为ASCII
            //string teststring = WordTo8bit01("I have a dog.");
            string[] sub = new string[bitString.Length / 8];
            int[] count = new int[bitString.Length / 8];
            for (int i = 0; i < bitString.Length / 8; i++)
            {
                sub[i] = bitString.Substring(i * 8, 8);
                count[i] = Convert.ToInt32(sub[i], 2);
                //Console.WriteLine("{0}ahifahisdfiasdhfasdfasdfasd", count[i]);
            }
            //将ASSII转化为字母\
            string strCharacter = null;
            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] >= 0 && count[i] <= 255)
                {
                    ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                    byte[] byteArray = new byte[] { (byte)count[i] };
                    strCharacter += asciiEncoding.GetString(byteArray);

                }
            }
            return strCharacter;
        }

        private int[] Encode(int N, int q, int[] r, int[] m, int[] h)
        {
            int[] e = new int[N];
            e = MultiPloy(h, r, N, q);
            for (int i = 0; i < N; i++)
            {
                e[i] = (e[i] + m[i]) % q;
                if (e[i] < 0)
                { e[i] = e[i] + q; }
            }

            return e;

        }

        /// <summary>
        /// 解密过程
        /// </summary>
        /// <param name="N"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="f"></param>
        /// <param name="Fp"></param>
        /// <param name="e">密文</param>
        /// <returns></returns>
        public int[] Decode(int N, int p, int q, int[] a, int[] Fp, int[] e)
        {
            //int[] a = new int[N];
            int[] b = new int[N];
            int[] d = new int[N];
            //a = MultiPloy(F, e, N, q);
            for (int i = 0; i < N; i++)
            {
                if (a[i] < -q / 2)
                {
                    a[i] = a[i] + q;
                }
                else if (a[i] > q / 2)
                {
                    a[i] = a[i] - q;
                }

            }
            for (int i = 0; i < N; i++)
            {
                b[i] = a[i] % p;
            }

            d = MultiPloy(b, Fp, N, p);
            for (int i = 0; i < N; i++)
            {
                if (d[i] > p / 2)
                { d[i] = d[i] - p; }
                if (d[i] < (-p / 2))
                { d[i] = p + d[i]; }
            }
            return d;


        }
        /// <summary>
        /// 多项式的乘积
        /// </summary>
        /// <param name="a">多项式a</param>
        /// <param name="b">多项式b</param>
        /// <param name="N">标准参数</param>
        /// <param name="m">模m</param>
        /// <returns></returns>
        public int[] MultiPloy(int[] a, int[] b, int N, int m)
        {
            int[] c = new int[N];
            // int[] c = new int[N];

            for (int i = (N - 1); i >= 0; i--)
            {
                c[i] = 0;
            }
            for (int i = (N - 1); i >= 0; i--)
            {
                int K = 0;
                for (int j = 0; j <= i; j++)
                {
                    c[i] += a[j] * b[i - j];
                    c[i] = c[i] % m;
                }
                for (int j = 1; j <= N - 1 - i; j++)
                {
                    K += a[i + j] * b[N - j];
                    K = K % m;
                }
                c[i] = c[i] += K;
                c[i] = c[i] % m;
            }

            return c;
        }


        /// <summary>
        /// 公匙生成
        /// </summary>
        /// <param name="N"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="Fp"></param>
        /// <param name="Fq"></param>
        /// <returns></returns>
        public int[] NTRUcreateKey(int N, int p, int q, int[] f, int[] g, int[] h, int[] Fp, int[] Fq)
        {
            // Fp = InversePoly(f, N, p);
            // Fq = InversePoly(f, N, q);
            h = MultiPloy(Fq, g, N, q);
            for (int i = 0; i < N; i++)
            {
                if (h[i] < 0)//若是负数的情况
                {
                    h[i] = h[i] + q;

                }
                h[i] = (h[i] * p) % q;
            }
            return h;

        }

        public int[] InversePoly(int[] a, int N, int q)
        {
            int k = 0;
            int[] b = new int[N];
            int[] c = new int[N];
            int[] f = new int[N];
            int[] g = new int[N + 1];


            f = a;
            g[N] = 1; g[0] = -1; b[0] = 1;
            while (true)
            {


                while (f[0] == 0)
                {
                    for (int i = 1; i <= f.Length - 1; i++)
                    {
                        f[i - 1] = f[i];
                    }
                    for (int i = c.Length - 1; i > 0; i--)
                    {
                        c[i] = c[i - 1];

                    }
                    f[f.Length - 1] = 0;
                    c[0] = 0;
                    k++;

                }


                if (deg(f) == 0)
                {
                    for (int i = 0; i < N; i++)
                    {
                        b[i] = Ix(f[0], q) * b[i];
                        b[i] = b[i] % q;

                    }
                    int[] b1 = new int[N];
                    for (int i = 0; i < N; i++)
                    {
                        b1[i] = b[i];
                        if (b1[i] < 0)
                        {
                            b1[i] = b1[i] + q;
                        }
                    }
                    if (k > N)
                    {

                        for (int i = 0; i < N; i++)
                        {

                            b[i] = b1[(k - N + i) % N];
                            // b[(i + k - N) % N] = (b1[(2 * k - 3*N + i) % N]);

                        }

                    }
                    else
                    {
                        for (int i = 0; i <= k - 1; i++)
                        {
                            b[N - k + i] = b[i] % q;

                        }
                        for (int i = k; i <= N - 1; i++)
                        {
                            b[i - k] = b[i] % q;

                        }
                    }
                    return b;
                }


                if (deg(f) < deg(g))
                {
                    int[] t = new int[N];
                    t = f;
                    f = g;
                    g = t;
                    t = b;
                    b = c;
                    c = t;
                }
                int u = 0;
                if (g[0] < 0)
                {
                    int t = 0;
                    t = g[0];
                    t = (t + q) % q;
                    int Inx = Ix(t, q);
                    u = (f[0] * Inx) % q;

                }
                else
                {
                    int Inx = Ix(g[0], q);
                    u = (f[0] * Inx) % q;

                }
                for (int i = 0; i < N; i++)
                {
                    f[i] = (f[i] - u * g[i]) % q;
                    b[i] = (b[i] - u * c[i]) % q;
                }

            }
            // return b;
        }

        public int Ix(int m, int n)//扩展的欧几里得算法
        {
            int a, b, c, d, t;
            int ap, bp;
            int q, r;



            ap = b = 1;
            a = bp = 0;
            c = m;
            d = n;

            while (d != 0)
            {
                q = c / d; //商
                r = c % d;  //余数

                c = d;
                d = r;

                t = ap;
                ap = a;
                a = t - q * a;
                t = bp;
                bp = b;
                b = t - q * b;
            }
            if (ap < 0)
            {
                ap = ap + n;
            }
            return ap;


        }

        /// <summary>
        /// 求多项式的度
        /// </summary>
        /// <param name="f">多项式的系数数组</param>
        /// <returns></returns>
        public int deg(int[] f)
        {
            int d = 0;
            if (f[f.Length - 1] != 0)
            {
                d = f.Length - 1;
            }
            else
            {
                for (int i = f.Length - 1; i >= 1; i--)
                {

                    if (f[i] == 0 && f[i - 1] != 0)
                    {
                        d = i - 1;
                        break;
                    }
                }
            }
            return d;
        }
    }
}
