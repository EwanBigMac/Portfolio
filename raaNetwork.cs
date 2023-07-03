using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
//using System.Net.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class raaCheckBox
{
    uint m_Id;
    bool m_State;
    string m_ShaderName;
    string m_sName;

    public raaCheckBox(uint id, bool state, string name, string shaderName)
    {
        m_Id = id;
        m_State = state;
        m_ShaderName = shaderName;
        m_sName = name;
    }
    public raaCheckBox(uint id, bool state, string name)
    {
        m_Id = id;
        m_State = state;
        m_sName = name;
    }

    public uint id()
    {
        return m_Id;
    }
    public bool state()
    {
        return m_State;
    }

    public string shader()
    {
        return m_ShaderName;
    }

    public void setState(bool s)
    {
        m_State = s;
    }

    public string name()
    {
        return m_sName;
    }
}


public class raaMessage
{
    private int m_Offset = 0;
    private int m_Size;
    public const ushort csm_usTypeUndefined = 0;
    public const ushort csm_usTypeInt = 1;
    public const ushort csm_usTypeShortInt = 2;
    public const ushort csm_usTypeUnsignedInt = 3;
    public const ushort csm_usTypeBool = 4;
    public const ushort csm_usTypeChar = 5;
    public const ushort csm_usTypeFloat = 6;
    public const ushort csm_usTypeDouble = 7;
    public const ushort csm_usTypeIntArray = 8;
    public const ushort csm_usTypeShortIntArray = 9;
    public const ushort csm_usTypeUnsignedIntArray = 10;
    public const ushort csm_usTypeBoolArray = 11;
    public const ushort csm_usTypeCharArray = 12; // uses string class so raaVItem
    public const ushort csm_usTypeFloatArray = 13;
    public const ushort csm_usTypeDoubleArray = 14;

    protected byte[] m_Data = null;
    protected List<byte> m_DataList = new List<byte>();

    public byte[] data()
    {
        if (m_Data == null || m_Data.Length != m_DataList.Count)
            m_Data = m_DataList.ToArray();

        m_Offset = 0;
        m_Size = m_Data.Length;

        return m_Data;
    }


    public void print()
    {
        string s = "Message\n";



        foreach (raaVariant v in m_lVariants)
        {
            switch (v.type())
            {
                case csm_usTypeInt:
                    s += "\tInt -> " + v.asInt();
                    break;
                case csm_usTypeShortInt:
                    s += "\tShort Int -> " + v.asShortInt();
                    break;
                case csm_usTypeUnsignedInt:
                    s += "\tUnsigned Int -> " + v.asUnsignedInt();
                    break;
                case csm_usTypeBool:
                    s += "\tBool -> " + v.asBool();
                    break;
                case csm_usTypeChar:
                    s += "\tChar -> " + v.asChar();
                    break;
                case csm_usTypeFloat:
                    s += "\tFloat -> " + v.asFloat();
                    break;
                case csm_usTypeDouble:
                    s += "\tDouble -> " + v.asDouble();
                    break;
                case csm_usTypeIntArray:
                    s += "\tInt Array-> [";
                    foreach (int i in v.asIntArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeShortIntArray:
                    s += "\tShort Int Array-> [";
                    foreach (short i in v.asShortIntArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeUnsignedIntArray:
                    s += "\tUnsigned Int Array-> [";
                    foreach (uint i in v.asUnsignedIntArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeBoolArray:
                    s += "\tBool Array-> [";
                    foreach (bool i in v.asBoolArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeCharArray:
                    s += "\tChar Array-> [";
                    s += v.asCharArray();
                    s += "]";
                    break;
                case csm_usTypeFloatArray:
                    s += "\tFloat Array-> [";
                    foreach (float i in v.asFloatArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeDoubleArray:
                    s += "\tInt Array-> [";
                    foreach (double i in v.asDoubleArray()) s += i + ", ";
                    s += "]";
                    break;
                case csm_usTypeUndefined:
                    s += "\tUndefined-> [";
                    foreach (byte i in v.asUndefined()) s += i + ", ";
                    s += "]";
                    break;
            }
            s += "\n";
        }
        s += "##End##";
        Debug.Log(s);
    }

    public class raaVariant
    {
        protected ushort m_usType;
        protected uint m_uiSize;

        public ushort type() { return m_usType; }
        public uint size() { return m_uiSize; }
        public virtual byte[] asUndefined() { return null; }
        public virtual int asInt() { return 0; }
        public virtual short asShortInt() { return 0; }
        public virtual uint asUnsignedInt() { return 0; }
        public virtual bool asBool() { return false; }
        public virtual char asChar() { return '¬'; }
        public virtual float asFloat() { return 0.0f; }
        public virtual double asDouble() { return 0.0; }
        public virtual int[] asIntArray() { return null; }
        public virtual short[] asShortIntArray() { return null; }
        public virtual uint[] asUnsignedIntArray() { return null; }
        public virtual bool[] asBoolArray() { return null; }
        public virtual string asCharArray() { return null; }
        public virtual float[] asFloatArray() { return null; }
        public virtual double[] asDoubleArray() { return null; }
    }

    public class raaVItem<T> : raaVariant
    {
        private T m_Value;

        public raaVItem(T value, ushort usType)
        {
            m_Value = value;
            m_uiSize = 1;
            m_usType = usType;
        }

        public T data() { return m_Value; }

        public override int asInt() { if (m_usType == csm_usTypeInt) return (int)(object)m_Value; else return 0; }
        public override short asShortInt() { if (m_usType == csm_usTypeShortInt) return (short)(object)m_Value; else return 0; }
        public override uint asUnsignedInt() { if (m_usType == csm_usTypeUnsignedInt) return (uint)(object)m_Value; else return 0; }
        public override bool asBool() { if (m_usType == csm_usTypeBool) return (bool)(object)m_Value; else return false; }
        public override char asChar() { if (m_usType == csm_usTypeChar) return (char)(object)m_Value; else return '¬'; }
        public override float asFloat() { if (m_usType == csm_usTypeFloat) return (float)(object)m_Value; else return 0.0f; }
        public override double asDouble() { if (m_usType == csm_usTypeDouble) return (double)(object)m_Value; else return 0.0; }
        public override string asCharArray() { if (m_usType == csm_usTypeCharArray) return (string)(object)m_Value; else return null; }
    }

    public class raaVArray<T> : raaVariant
    {
        private T[] m_Value = null;

        public raaVArray(T[] value, ushort usType)
        {
            m_Value = value;
            m_uiSize = (uint)value.Length;
            m_usType = usType;
        }

        public T[] data() { return m_Value; }

        public override int[] asIntArray() { if (m_usType == csm_usTypeIntArray) return (int[])(object)m_Value; else return null; }
        public override short[] asShortIntArray() { if (m_usType == csm_usTypeShortIntArray) return (short[])(object)m_Value; else return null; }
        public override uint[] asUnsignedIntArray() { if (m_usType == csm_usTypeUnsignedIntArray) return (uint[])(object)m_Value; else return null; }
        public override bool[] asBoolArray() { if (m_usType == csm_usTypeBoolArray) return (bool[])(object)m_Value; else return null; }
        public override float[] asFloatArray() { if (m_usType == csm_usTypeFloatArray) return (float[])(object)m_Value; else return null; }
        public override double[] asDoubleArray() { if (m_usType == csm_usTypeDoubleArray) return (double[])(object)m_Value; else return null; }
    }

    public class raaVUndefined : raaVariant
    {
        private byte[] m_Data;

        public raaVUndefined(byte[] value)
        {
            m_Data = value;
            m_uiSize = (uint)value.Length;
            m_usType = csm_usTypeUndefined;
        }

        public byte[] data() { return m_Data; }
        public override byte[] asUndefined() { return m_Data; }
    }

    protected List<raaVariant> m_lVariants = new List<raaVariant>();

    public List<raaVariant> variants()
    {
        return m_lVariants;
    }

    void clear() { m_lVariants.Clear(); Array.Clear(m_Data, 0, m_Data.Length); }

    List<raaVariant> items()
    {
        return m_lVariants;
    }

    protected void unpack()
    {
        m_lVariants.Clear();

        if (m_Data != null)
        {
            int i = m_Offset;
            while (i < (m_Size + m_Offset))
            {
                ushort usType = BitConverter.ToUInt16(m_Data, i);
                i += sizeof(ushort);
                uint uiSize = 0;
                switch (usType)
                {
                    case csm_usTypeInt:
                        add((int)BitConverter.ToInt32(m_Data, i));
                        i += sizeof(int);
                        break;
                    case csm_usTypeShortInt:
                        add((short)BitConverter.ToInt16(m_Data, i));
                        i += sizeof(short);
                        break;
                    case csm_usTypeUnsignedInt:
                        add((uint)BitConverter.ToUInt32(m_Data, i));
                        i += sizeof(uint);
                        break;
                    case csm_usTypeBool:
                        add((bool)BitConverter.ToBoolean(m_Data, i));
                        i += sizeof(bool);
                        break;
                    case csm_usTypeChar:
                        add((char)BitConverter.ToChar(m_Data, i));
                        i += sizeof(char);
                        break;
                    case csm_usTypeFloat:
                        add((float)BitConverter.ToSingle(m_Data, i));
                        i += sizeof(float);
                        break;
                    case csm_usTypeDouble:
                        add((double)BitConverter.ToDouble(m_Data, i));
                        i += sizeof(double);
                        break;
                    case csm_usTypeIntArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        int[] ai = new int[uiSize];
                        Buffer.BlockCopy(m_Data, i, ai, 0, (int)uiSize * sizeof(int));
                        add(ai);
                        i += (sizeof(int) * (int)uiSize);
                        break;
                    case csm_usTypeShortIntArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        short[] asi = new short[uiSize];
                        Buffer.BlockCopy(m_Data, i, asi, 0, (int)uiSize * sizeof(short));
                        add(asi);
                        i += (sizeof(short) * (int)uiSize);
                        break;
                    case csm_usTypeUnsignedIntArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        uint[] aui = new uint[uiSize];
                        Buffer.BlockCopy(m_Data, i, aui, 0, (int)uiSize * sizeof(uint));
                        add(aui);
                        i += (sizeof(uint) * (int)uiSize);
                        break;
                    case csm_usTypeBoolArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        bool[] ab = new bool[uiSize];
                        Buffer.BlockCopy(m_Data, i, ab, 0, (int)uiSize * sizeof(bool));
                        add(ab);
                        i += (sizeof(bool) * (int)uiSize);
                        break;
                    case csm_usTypeCharArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        string ac = Encoding.ASCII.GetString(m_Data, i, (int)uiSize);
                        add(ac);
                        i += (sizeof(char) * (int)uiSize);
                        break;
                    case csm_usTypeFloatArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        float[] af = new float[uiSize];
                        Buffer.BlockCopy(m_Data, i, af, 0, (int)uiSize * sizeof(float));
                        add(af);
                        i += (sizeof(float) * (int)uiSize);
                        break;
                    case csm_usTypeDoubleArray:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        double[] ad = new double[uiSize];
                        Buffer.BlockCopy(m_Data, i, ad, 0, (int)uiSize * sizeof(double));
                        add(ad);
                        i += (sizeof(double) * (int)uiSize);
                        break;
                    case csm_usTypeUndefined:
                        uiSize = BitConverter.ToUInt32(m_Data, i);
                        i += sizeof(uint);
                        byte[] abyte = new byte[uiSize];
                        Buffer.BlockCopy(m_Data, i, abyte, 0, (int)uiSize);
                        add(abyte);
                        i += (int)uiSize;
                        break;
                }
            }
        }
    }

    public raaMessage()
    {
    }

    public raaMessage(byte[] data)
    {
        m_Offset = 0;
        m_Size = data.Length;
        m_Data = data;
        unpack();
    }
    public raaMessage(byte[] data, int offset, int size)
    {
        m_Offset = offset;
        m_Size = size;
        m_Data = data;
        unpack();
        print();
    }

    public void add(int i)
    {
        m_lVariants.Add(new raaVItem<int>(i, csm_usTypeInt));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeInt));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(short i)
    {
        m_lVariants.Add(new raaVItem<short>(i, csm_usTypeShortInt));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeShortInt));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(uint i)
    {
        m_lVariants.Add(new raaVItem<uint>(i, csm_usTypeUnsignedInt));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeUnsignedInt));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(bool i)
    {
        m_lVariants.Add(new raaVItem<bool>(i, csm_usTypeBool));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeBool));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(char i)
    {
        m_lVariants.Add(new raaVItem<char>(i, csm_usTypeChar));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeChar));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(float i)
    {
        m_lVariants.Add(new raaVItem<float>(i, csm_usTypeFloat));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeFloat));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(double i)
    {
        m_lVariants.Add(new raaVItem<double>(i, csm_usTypeDouble));
        m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeDouble));
        m_DataList.AddRange(BitConverter.GetBytes(i));
    }
    public void add(int[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<int>(i, csm_usTypeIntArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeIntArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(int)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }
    public void add(short[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<short>(i, csm_usTypeShortIntArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeShortIntArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(short)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }

    public void add(uint[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<uint>(i, csm_usTypeUnsignedIntArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeUnsignedIntArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(uint)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }

    public void add(string i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVItem<string>(i, csm_usTypeCharArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeCharArray));
            byte[] b = Encoding.Default.GetBytes(i);
            m_DataList.AddRange(BitConverter.GetBytes((uint)b.Length));
            m_DataList.AddRange(b);
        }
    }

    public void add(bool[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<bool>(i, csm_usTypeBoolArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeBoolArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(bool)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }

    public void add(float[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<float>(i, csm_usTypeFloatArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeFloatArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(float)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }

    public void add(double[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVArray<double>(i, csm_usTypeDoubleArray));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeDoubleArray));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            byte[] a = new byte[i.Length * sizeof(double)];
            Buffer.BlockCopy(i, 0, a, 0, a.Length);
            m_DataList.AddRange(a);
        }
    }

    public void add(byte[] i)
    {
        if (i != null)
        {
            m_lVariants.Add(new raaVUndefined(i));
            m_DataList.AddRange(BitConverter.GetBytes(csm_usTypeUndefined));
            m_DataList.AddRange(BitConverter.GetBytes((uint)i.Length));
            m_DataList.AddRange(i);
        }
    }
}

public class raaEvent
{
    public const ushort cm_usConnect = 0;
    public const ushort cm_usDisconnect = 1;
    public const ushort cm_usMsg = 2;
    public const ushort cm_usUndefined = 3;

    private raaMessage m_Msg = null;
    private ushort m_Type = cm_usUndefined;
    public raaMessage msg()
    {
        return m_Msg;
    }

    public raaEvent(raaMessage m)
    {
        m_Msg = m;
        m_Type = cm_usMsg;
    }

    public raaEvent(ushort e)
    {
        m_Type = e;
    }

    public ushort type()
    {
        return m_Type;
    }
}

public class raaAsyncServer
{
    Socket server;
    Socket client;
    byte[] byteData = new byte[4096];

    public delegate void incommingEvent(raaEvent e);
    protected List<raaEvent> events = new List<raaEvent>();
    private static Mutex mut = new Mutex();



    incommingEvent ieCallback = null;

    public void processEvents()
    {
        List<raaEvent> localEvents = null;
        if (mut.WaitOne())
        {
            if (events.Count > 0)
            {
                localEvents = new List<raaEvent>();

                foreach (raaEvent e in events) localEvents.Add(e);
                events.Clear();
            }
            mut.ReleaseMutex();
        }

        if (localEvents != null)
        {
            if (ieCallback != null)
            {
                foreach (raaEvent e in localEvents)
                {
                    if(e.msg()!=null) e.msg().print();
                    else Debug.Log("Empty msg -> "+e.type());

                    ieCallback(e);
                }
            }
        }
    }


    private void onRecieve(IAsyncResult ar)
    {
        try
        {
            Socket clientSocket = (Socket)ar.AsyncState;
            int i = clientSocket.EndReceive(ar);
            if (i > 0)
            {
                ushort usType = BitConverter.ToUInt16(byteData, 0);
                int iSize = BitConverter.ToInt32(byteData, 2);
                uint uiID = BitConverter.ToUInt32(byteData, 6);

                Debug.Log(iSize.ToString() +" , "+ usType +", " + uiID);

                if (usType == raaEvent.cm_usDisconnect)
                {
                    if (mut.WaitOne())
                    {
                        events.Add(new raaEvent(raaEvent.cm_usDisconnect));
                        mut.ReleaseMutex();
                    }
                }
                else if (usType == raaEvent.cm_usMsg)
                {
                    if (mut.WaitOne())
                    {
                        events.Add(new raaEvent(new raaMessage(byteData, 10, iSize)));
                        mut.ReleaseMutex();
                    }
                }
            }
            client.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(onRecieve), client);
        }
        catch (Exception ex)
        {
            Debug.Log("SGSserverTCP onRecieve - exception" + ex.Message);
        }

    }

    public void onSend(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);
            Debug.Log("Message Sent");
        }
        catch (Exception ex)
        {
            Debug.Log("SGSserverTCP onSend - exception "+ ex.Message);
        }
    }

    public void send(raaMessage m)
    {
        if (client != null && m != null)
        {
            ushort msgType = 2;
            int size = m.data().Length;
            uint uid = 0;

            byte[] b = new byte[10 + m.data().Length];
            Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, b, 0, sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(size), 0, b, sizeof(ushort), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(uid), 0, b, sizeof(ushort) + sizeof(int), sizeof(uint));
            Buffer.BlockCopy(m.data(), 0, b, 10, m.data().Length);

            client.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(onSend), client);
        }
    }

    private void onDisconnect(IAsyncResult ar)
    {
        client.EndDisconnect(ar);
        client = null;
    }
    private void onAccept(IAsyncResult ar)
    {
        try
        {
            client = server.EndAccept(ar);
            Debug.Log("Accepted");
            //Start listening for more clients
            server.BeginAccept(new AsyncCallback(onAccept), null);

            if (mut.WaitOne())
            {
                Debug.Log("Message sent");
                events.Add(new raaEvent(raaEvent.cm_usConnect));
                mut.ReleaseMutex();
            }

            //Once the client connects then start 
            //receiving the commands from her
            client.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(onRecieve), client);
            //            client.BeginDisconnect(true, new AsyncCallback(onDisconnect), client);
        }
        catch (Exception ex)
        {
            Debug.Log("SGSserverTCP onAccept - exception "+ex.Message);
        }
    }

    public raaAsyncServer(int port, incommingEvent ie)
    {
        ieCallback = ie;
        try
        {
            Debug.Log("Server Started");
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            server.Bind(ip);
            server.Listen(4);
            server.BeginAccept(new AsyncCallback(onAccept), null);
        }
        catch (SocketException sx)
        {
            Debug.Log("SGSserverTCP - exception -> " + sx.Message);
        }
        catch (Exception ex)
        {
            Debug.Log("SGSserverTCP - exception "+ex.Message);
        }
    }
}


public class raaServer
{
    #region private members 	
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    #endregion

    protected List<raaEvent> events = new List<raaEvent>();
    private static Mutex mut = new Mutex();
    private int m_Port = 19723;

    public raaServer(int port)
    {
        m_Port = port;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }
    public raaServer()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    public void close()
    {
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Abort();
        }
    }

    void start()
    {
        if (tcpListenerThread == null)
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();

        }
    }
    private bool read(NetworkStream s, ref byte[] b)
    {
        int pos = 0;
        int count = b.Length;

        //        if(s.DataAvailable())

        while (true)
        {
            try
            {
                int i = s.Read(b, pos, count);

                if (i == 0) return false;
                else
                {
                    count -= i;
                    pos += i;
                }

                if (count == 0) return true;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
        }
    }

    public delegate void incommingEvent(raaEvent e);

    public void processEvents(incommingEvent ie)
    {
        List<raaEvent> localEvents = null;
        if (mut.WaitOne())
        {
            if (events.Count > 0)
            {
                localEvents = new List<raaEvent>();

                foreach (raaEvent e in events) localEvents.Add(e);
                events.Clear();
            }
            mut.ReleaseMutex();
        }

        if (localEvents != null)
        {
            foreach (raaEvent e in localEvents)
            {
                ie(e);
            }
        }
    }

    private void ListenForIncommingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, m_Port);
            tcpListener.Start();
            Byte[] hBytes = new Byte[10];
            Byte[] mBytes = null;

            while (true)
            {
                mBytes = null;
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    Debug.Log("Accepted Client");
                    NetworkStream stream = connectedTcpClient.GetStream();
                    try
                    {

                        if (mut.WaitOne())
                        {
                            events.Add(new raaEvent(raaEvent.cm_usConnect));
                            mut.ReleaseMutex();
                        }
                        Debug.Log("About to read");

                        while (read(stream, ref hBytes))
                        {
                            ushort usType = BitConverter.ToUInt16(hBytes, 0);
                            int iSize = BitConverter.ToInt32(hBytes, 2);
                            uint uiID = BitConverter.ToUInt32(hBytes, 6);

                            if (iSize != 0)
                            {
                                mBytes = new byte[iSize];

                                if (read(stream, ref mBytes))
                                {
                                    if (mut.WaitOne())
                                    {
                                        events.Add(new raaEvent(new raaMessage(mBytes)));
                                        mut.ReleaseMutex();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null) ((IDisposable)stream).Dispose();
                        if (mut.WaitOne())
                        {
                            events.Add(new raaEvent(raaEvent.cm_usDisconnect));
                            mut.ReleaseMutex();
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    public void SendMessage(raaMessage m)
    {
        if (connectedTcpClient == null) return;

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                ushort msgType = 2;
                int size = m.data().Length;
                uint uid = 0;
                byte[] b = new byte[10];

                Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, b, 0, sizeof(ushort));
                Buffer.BlockCopy(BitConverter.GetBytes(size), 0, b, sizeof(ushort), sizeof(int));
                Buffer.BlockCopy(BitConverter.GetBytes(uid), 0, b, sizeof(ushort) + sizeof(int), sizeof(uint));
                stream.Write(b, 0, b.Length);
                stream.Write(m.data(), 0, size);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


}

public class raaInitMessage : raaMessage
{
    const short cm_sSlider = 0;
    const short cm_sIntSlider = 1;
    const short cm_sRangeSlider = 2;
    const short cm_sRangeSliderDefinedColour = 3;
    const short cm_sRangeSliderCombo = 4;
    const short cm_sRangeSliderDefinedColourCombo = 5;
    const short cm_sCheckBoxGroup = 6;

    public raaInitMessage()
    {
        add((short)1);
    }

    public void addSlider(string name, uint id, float min, float max, float value)
    {
        add(cm_sSlider);
        add(id);
        add(name);
        add(min);
        add(max);
        add(value);
    }
    public void addSlider(string name, uint id, int min, int max, int value)
    {
        add(cm_sIntSlider);
        add(id);
        add(name);
        add((int)min);
        add((int)max);
        add((int)value);
    }
    public void addSlider(string name, uint id, float min, float max, float lowerValue, float upperValue, bool enabled, bool canChangeCol)
    {
        add(cm_sRangeSlider);
        add(id);
        add(name);
        add((float)min);
        add((float)max);
        add((float)lowerValue);
        add((float)upperValue);
        add(enabled);
        add(canChangeCol);
    }
    public void addSlider(string name, uint id, float min, float max, float lowerValue, float upperValue, bool enabled, float[] col, bool canChangeCol)
    {
        add(cm_sRangeSliderDefinedColour);
        add(id);
        add(name);
        add((float)min);
        add((float)max);
        add((float)lowerValue);
        add((float)upperValue);
        add(enabled);
        add(col);
        add(canChangeCol);
    }
    public void addSlider(string name, uint id, float min, float max, float lowerValue, float upperValue, bool canChangeCol, int select, string[] modes)
    {
        add(cm_sRangeSliderCombo);
        add(id);
        add(name);
        add((float)min);
        add((float)max);
        add((float)lowerValue);
        add((float)upperValue);
        add(canChangeCol);
        add(select);
        add((int)modes.Length);
        foreach (string s in modes) add(s);
    }
    public void addSlider(string name, uint id, float min, float max, float lowerValue, float upperValue, float[] col, bool canChangeCol, int select, string[] modes)
    {
        add(cm_sRangeSliderDefinedColourCombo);
        add(id);
        add(name);
        add((float)min);
        add((float)max);
        add((float)lowerValue);
        add((float)upperValue);
        add(col);
        add(canChangeCol);
        add(select);
        add((int)modes.Length);
        foreach (string s in modes) add(s);
    }

    public void addCheckBoxGroup(string name, uint id, Dictionary<uint, raaCheckBox> checkboxes)
    {
        if (checkboxes.Count > 0)
        {
            add(cm_sCheckBoxGroup);
            add(id);
            add(name);
            add(checkboxes.Count);

            foreach(raaCheckBox cb in checkboxes.Values)
            {
                add(cb.id());
                add(cb.name());
                add(cb.state());
            }
        }
    }
}

public class raaMessageParser
{
    const short cm_sSlider = 0;
    const short cm_sIntSlider = 1;
    const short cm_sRangeSlider = 2;
    const short cm_sRangeSliderDefinedColour = 3;
    const short cm_sRangeSliderCombo = 4;
    const short cm_sRangeSliderDefinedColourCombo = 5;
    const short cm_sCheckBoxGroup = 6;

    const short cm_sRangeSliderValue = 0;
    const short cm_sRangeSliderState = 1;
    const short cm_sRangeSliderColour = 2;

    public delegate void sliderEvent(uint id, float value);
    public delegate void sliderIntEvent(uint id, int value);
    public delegate void rangeSliderValueEvent(uint id, float fLower, float fUpper);
    public delegate void rangeSliderStateEvent(uint id, bool bState);
    public delegate void rangeSliderComboEvent(uint id, int iState);
    public delegate void rangeSliderColourEvent(uint id, float[] col);
    public delegate void checkBoxEvent(uint groupId, uint boxId, bool bState);

    protected sliderEvent sliderFunction = null;
    protected sliderIntEvent sliderIntFunction = null;
    protected rangeSliderValueEvent rangeSliderValueFunction = null;
    protected rangeSliderStateEvent rangeSliderStateFunction = null;
    protected rangeSliderComboEvent rangeSliderComboFunction = null;
    protected rangeSliderColourEvent rangeSliderColourFunction = null;
    protected checkBoxEvent checkBoxFunction = null;

    public raaMessageParser()
    {
        sliderFunction = sliderPrintEvent;
        sliderIntFunction = sliderIntPrintEvent;
        rangeSliderValueFunction = rangeSliderValuePrintEvent;
        rangeSliderStateFunction = rangeSliderStatePrintEvent;
        rangeSliderComboFunction = rangeSliderComboPrintEvent;
        rangeSliderColourFunction = rangeSliderColourPrintEvent;
        checkBoxFunction = checkBoxPrintEvent;
    }
    public raaMessageParser(sliderEvent sf, sliderIntEvent si, rangeSliderValueEvent rsv, rangeSliderStateEvent rss, rangeSliderComboEvent rcs, rangeSliderColourEvent rsc)
    {
        sliderFunction = sf;
        sliderIntFunction = si;
        rangeSliderValueFunction = rsv;
        rangeSliderStateFunction = rss;
        rangeSliderComboFunction = rcs;
        rangeSliderColourFunction = rsc;
    }

    public void setSliderEventFunction(sliderEvent sf)
    {
        sliderFunction = sf;
    }
    public void setSliderIntEventFunction(sliderIntEvent si)
    {
        sliderIntFunction = si;
    }

    public void setRangeSliderValueEventFunction(rangeSliderValueEvent rsv)
    {
        rangeSliderValueFunction = rsv;
    }
    public void setRangeSliderStateEventFunction(rangeSliderStateEvent rss)
    {
        rangeSliderStateFunction = rss;
    }
    public void setRangeSliderComboEventFunction(rangeSliderComboEvent rcs)
    {
        rangeSliderComboFunction = rcs;
    }
    public void setRangeSliderColourEventFunction(rangeSliderColourEvent rsc)
    {
        rangeSliderColourFunction = rsc;
    }
    public void setCheckBoxEventFunction(checkBoxEvent cb)
    {
        checkBoxFunction = cb;
    }

    public void parse(raaMessage m)
    {
        raaMessage.raaVariant[] v = m.variants().ToArray();

        switch (v[0].asShortInt())
        {
            case cm_sSlider:
                {
                    uint id = v[1].asUnsignedInt();
                    float f = v[2].asFloat();
                    if (sliderFunction != null) sliderFunction(id, f);
                }
                break;
            case cm_sIntSlider:
                {
                    uint id = v[1].asUnsignedInt();
                    int i = v[2].asInt();
                    if (sliderIntFunction != null) sliderIntFunction(id, i);
                }
                break;
            case cm_sRangeSlider:
            case cm_sRangeSliderDefinedColour:
                switch (v[1].asShortInt())
                {
                    case cm_sRangeSliderValue:
                        {
                            uint id = v[2].asUnsignedInt();
                            float l = v[3].asFloat();
                            float u = v[4].asFloat();

                            Debug.Log("RangeSlider");
                            if (rangeSliderValueFunction != null) rangeSliderValueFunction(id, l, u);
                        }
                        break;
                    case cm_sRangeSliderState:
                        {
                            uint id = v[2].asUnsignedInt();
                            bool b = v[3].asBool();
                            if (rangeSliderStateFunction != null) rangeSliderStateFunction(id, b);
                        }
                        break;
                    case cm_sRangeSliderColour:
                        {
                            uint id = v[2].asUnsignedInt();
                            float[] c = v[3].asFloatArray();
                            if (rangeSliderColourFunction != null) rangeSliderColourFunction(id, c);
                        }
                        break;
                }
                break;
            case cm_sRangeSliderCombo:
            case cm_sRangeSliderDefinedColourCombo:
                switch (v[1].asShortInt())
                {
                    case cm_sRangeSliderValue:
                        {
                            uint id = v[2].asUnsignedInt();
                            float l = v[3].asFloat();
                            float u = v[4].asFloat();
                            if (rangeSliderValueFunction != null) rangeSliderValueFunction(id, l, u);
                        }
                        break;
                    case cm_sRangeSliderState:
                        {
                            uint id = v[2].asUnsignedInt();
                            int state = v[3].asInt();
                            if (rangeSliderStateFunction != null) rangeSliderComboFunction(id, state);
                        }
                        break;
                    case cm_sRangeSliderColour:
                        {
                            uint id = v[2].asUnsignedInt();
                            float[] c = v[3].asFloatArray();
                            if (rangeSliderColourFunction != null) rangeSliderColourFunction(id, c);
                        }
                        break;
                }

                break;
            case cm_sCheckBoxGroup:
                {
                    uint groupId = v[1].asUnsignedInt();
                    uint boxId = v[2].asUnsignedInt();
                    bool state = v[3].asBool();
                }
                break;
        }
    }

    void sliderPrintEvent(uint id, float value)
    {
        Debug.Log("Slider Event::" + id + " -> " + value);
    }

    void sliderIntPrintEvent(uint id, int value)
    {
        Debug.Log("Slider Int Event::" + id + " -> " + value);
    }

    void rangeSliderValuePrintEvent(uint id, float lower, float upper)
    {
        Debug.Log("Range Slider Value Event::" + id + " -> (" + lower + "::" + upper + ")");
    }
    void rangeSliderStatePrintEvent(uint id, bool bState)
    {
        if (bState)
            Debug.Log("Range Slider State Event::Enabled");
        else
            Debug.Log("Range Slider State Event::Disabled");
    }
    void rangeSliderComboPrintEvent(uint id, int iState)
    {
        Debug.Log("Range Slider Combo Event::" + iState);
    }
    void rangeSliderColourPrintEvent(uint id, float[] col)
    {
        Debug.Log("Range Slider Colour Event::" + id + " -> (" + col[0] + ":" + col[1] + ":" + col[2] + ":" + col[3] + ")");
    }
    void checkBoxPrintEvent(uint groupId, uint boxId, bool state)
    {
        Debug.Log("CheckBox Event::" + groupId + "::" + boxId + " -> " + (state?"true":"false"));
    }

}

public class raaInterfaceManager : raaMessageParser
{
    class raaInterfaceItem
    {
/*        public delegate void localSliderEvent(int id, float value);
        public delegate void localSliderIntEvent(int id, int value);
        public delegate void localRangeSliderValueEvent(int id, float fLower, float fUpper);
        public delegate void localRangeSliderStateEvent(int id, bool bState);
        public delegate void localRangeSliderComboEvent(int id, int iState);
        public delegate void localRangeSliderColourEvent(int id, float[] col);
*/
/*
        protected sliderEvent sliderFunction = null;
        protected sliderIntEvent sliderIntFunction = null;
        protected rangeSliderValueEvent rangeSliderValueFunction = null;
        protected rangeSliderStateEvent rangeSliderStateFunction = null;
        protected rangeSliderComboEvent rangeSliderComboFunction = null;
        protected rangeSliderColourEvent rangeSliderColourFunction = null;
*/

        public enum Types { Slider, IntSlider, RangeSlider, RangeSliderCombo, CheckBoxGroup };
        public string m_DisplayName { get; }
        public string m_ShaderName { get; }
        public Types m_Type { get; }
        public uint m_Id { get; }

        private static uint sm_Id = 0;

        static public Renderer m_Renderer { get; set; }



        public raaInterfaceItem(string displayName, string shaderName, Types type)
        {
            m_DisplayName = displayName;
            m_ShaderName = shaderName;
            m_Type = type;
            m_Id = sm_Id++;
        }
        public raaInterfaceItem(string displayName, Types type)
        {
            m_DisplayName = displayName;
            m_ShaderName = null;
            m_Type = type;
            m_Id = sm_Id++;
        }

        public virtual void addItem(ref raaInitMessage m) { }
        public virtual void apply(float value) { }
        public virtual void apply(int value) { }
        public virtual void apply(float fLower, float fUpper) { }
        public virtual void apply(bool b) { }
        public virtual void apply(float[] colour) { }
        public virtual void apply(uint id0, uint id1, bool state) { }

        public virtual void intialise() { }

    }

    class raaInterfaceItemSlider : raaInterfaceItem
    {
        float m_fValue;
        float m_fMin;
        float m_fMax;
        sliderEvent localFunction = null;



        public raaInterfaceItemSlider(string displayName, string shaderName, float fValue, float fMin, float fMax) : base(displayName, shaderName, Types.Slider)
        {
            m_fValue = fValue;
            m_fMin = fMin;
            m_fMax = fMax;
        }
        public raaInterfaceItemSlider(string displayName, sliderEvent eventFunction, float fValue, float fMin, float fMax) : base(displayName, Types.Slider)
        {
            m_fValue = fValue;
            m_fMin = fMin;
            m_fMax = fMax;
            localFunction = eventFunction;
        }

        public override void apply(float value)
        {
            m_fValue = value;
            if (m_ShaderName != null) m_Renderer.material.SetFloat(m_ShaderName, value);
            else if(localFunction!=null) localFunction(m_Id, value);
        }

        public override void intialise()
        {
            if (m_ShaderName != null) m_Renderer.material.SetFloat(m_ShaderName, m_fValue);
            else if(localFunction!=null) localFunction(m_Id, m_fValue);
        }

        public override void addItem(ref raaInitMessage m)
        {
            m.addSlider(m_DisplayName, m_Id, m_fMin, m_fMax, m_fValue);
        }
    }
    class raaInterfaceItemSliderInt : raaInterfaceItem
    {
        int m_iValue;
        int m_iMin;
        int m_iMax;
        sliderIntEvent localFunction = null;

        public raaInterfaceItemSliderInt(string displayName, string shaderName, int iValue, int iMin, int iMax) : base(displayName, shaderName, Types.IntSlider)
        {
            m_iValue = iValue;
            m_iMin = iMin;
            m_iMax = iMax;
        }
        public raaInterfaceItemSliderInt(string displayName, sliderIntEvent eventFunction, int iValue, int iMin, int iMax) : base(displayName, Types.IntSlider)
        {
            m_iValue = iValue;
            m_iMin = iMin;
            m_iMax = iMax;
            localFunction = eventFunction;
        }

        public override void apply(int value)
        {
            m_iValue = value;
            if (m_ShaderName != null) m_Renderer.material.SetInt(m_ShaderName, m_iValue);
            else if(localFunction!=null) localFunction(m_Id, m_iValue);
        }

        public override void intialise()
        {
            if (m_ShaderName != null) m_Renderer.material.SetInt(m_ShaderName, m_iValue);
            else if(localFunction!=null) localFunction(m_Id, m_iValue);
        }

        public override void addItem(ref raaInitMessage m)
        {
            m.addSlider(m_DisplayName, m_Id, m_iMin, m_iMax, m_iValue);
        }
    }

    class raaInterfaceItemRangeSlider : raaInterfaceItem
    {
        float[] m_afValues = new float[2];
        float m_fUpper;
        float m_fMin;
        float m_fMax;
        bool m_bState;
        bool m_bUseColour;
        float[] m_Colour = new float[4];
        rangeSliderValueEvent localValueEvent = null;
        rangeSliderStateEvent localStateEvent = null;
        rangeSliderColourEvent localColourEvent = null;

        public raaInterfaceItemRangeSlider(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, bool bState) : base(displayName, shaderName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_bState = bState;
            m_bUseColour = false;
        }
        public raaInterfaceItemRangeSlider(string displayName, rangeSliderValueEvent valueEvent, rangeSliderStateEvent stateEvent, float fLower, float fUpper, float fMin, float fMax, bool bState) : base(displayName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_bState = bState;
            m_bUseColour = false;
            localStateEvent = stateEvent;
            localValueEvent = valueEvent;

        }
        public raaInterfaceItemRangeSlider(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, bool bState, bool bUseColour, float[] colour) : base(displayName, shaderName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_bState = bState;
            m_bUseColour = bUseColour;
            m_Colour[0] = colour[0];
            m_Colour[1] = colour[1];
            m_Colour[2] = colour[2];
            m_Colour[3] = colour[3];
        }
        public raaInterfaceItemRangeSlider(string displayName, rangeSliderValueEvent valueEvent, rangeSliderStateEvent stateEvent, rangeSliderColourEvent colourEvent, float fLower, float fUpper, float fMin, float fMax, bool bState, bool bUseColour, float[] colour) : base(displayName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_bState = bState;
            m_bUseColour = bUseColour;
            m_Colour[0] = colour[0];
            m_Colour[1] = colour[1];
            m_Colour[2] = colour[2];
            m_Colour[3] = colour[3];
            localStateEvent = stateEvent;
            localValueEvent = valueEvent;
            localColourEvent = colourEvent;
        }

        public override void apply(float fLower, float fUpper)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;

            Debug.Log(m_ShaderName + " Values [" + m_afValues[0] + ", " + m_afValues[1] + "]");

            if (m_ShaderName != null) m_Renderer.material.SetFloatArray(m_ShaderName, m_afValues);
            else if(localValueEvent!=null) localValueEvent(m_Id, fLower, fUpper);
        }
        public override void apply(bool bState)
        {
            m_bState = bState;

            if (m_ShaderName != null) m_Renderer.material.SetInt(m_ShaderName + "B", m_bState ? 1 : 0);
            else if(localStateEvent!=null) localStateEvent(m_Id, m_bState);
        }
        public override void apply(float[] col)
        {
            m_Colour[0] = col[0];
            m_Colour[1] = col[1];
            m_Colour[2] = col[2];
            m_Colour[3] = col[3];
            if (m_ShaderName != null) m_Renderer.material.SetFloatArray(m_ShaderName + "C", m_Colour);
            else if(localColourEvent!=null) localColourEvent(m_Id, m_Colour);
        }
        public override void intialise()
        {
            if (m_ShaderName != null)
            {
                m_Renderer.material.SetFloatArray(m_ShaderName, m_afValues);
                m_Renderer.material.SetInt(m_ShaderName + "B", m_bState ? 1 : 0);
                m_Renderer.material.SetFloatArray(m_ShaderName + "C", m_Colour);
            }
            else
            {
                if (localValueEvent != null) localValueEvent(m_Id, m_afValues[0], m_afValues[1]);
                if (localColourEvent != null) localColourEvent(m_Id, m_Colour);
                if (localStateEvent != null) localStateEvent(m_Id, m_bState);
            }
        }
        public override void addItem(ref raaInitMessage m)
        {
            if (m_bUseColour)
                m.addSlider(m_DisplayName, m_Id, m_fMin, m_fMax, m_afValues[0], m_afValues[1], m_bState, m_Colour, true);
            else
                m.addSlider(m_DisplayName, m_Id, m_fMin, m_fMax, m_afValues[0], m_afValues[1], m_bState, false);
        }
    }

    class raaInterfaceItemRangeSliderCombo : raaInterfaceItem
    {
        float[] m_afValues = new float[2];
        float m_fUpper;
        float m_fMin;
        float m_fMax;
        int m_iState;
        bool m_bUseColour;
        float[] m_Colour = new float[4];
        string[] m_Modes = null;
        rangeSliderValueEvent localValueEvent = null;
        rangeSliderComboEvent localComboEvent = null;
        rangeSliderColourEvent localColourEvent = null;


        public raaInterfaceItemRangeSliderCombo(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes) : base(displayName, shaderName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_iState = iState;
            m_Modes = modes;
            m_bUseColour = false;
        }
        public raaInterfaceItemRangeSliderCombo(string displayName, rangeSliderValueEvent valueEvent, rangeSliderComboEvent comboEvent, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes) : base(displayName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_iState = iState;
            m_Modes = modes;
            m_bUseColour = false;
            localComboEvent = comboEvent;
            localValueEvent = valueEvent;

        }
        public raaInterfaceItemRangeSliderCombo(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes, bool bUseColour, float[] colour) : base(displayName, shaderName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_iState = iState;
            m_Modes = modes;
            m_bUseColour = bUseColour;
            m_Colour[0] = colour[0];
            m_Colour[1] = colour[1];
            m_Colour[2] = colour[2];
            m_Colour[3] = colour[3];
        }
        public raaInterfaceItemRangeSliderCombo(string displayName, rangeSliderValueEvent valueEvent, rangeSliderComboEvent comboEvent, rangeSliderColourEvent colourEvent, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes, bool bUseColour, float[] colour) : base(displayName, Types.RangeSlider)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            m_fMin = fMin;
            m_fMax = fMax;
            m_iState = iState;
            m_Modes = modes;
            m_bUseColour = bUseColour;
            m_Colour[0] = colour[0];
            m_Colour[1] = colour[1];
            m_Colour[2] = colour[2];
            m_Colour[3] = colour[3];
            localComboEvent = comboEvent;
            localValueEvent = valueEvent;
            localColourEvent = colourEvent;

        }

        public override void apply(float fLower, float fUpper)
        {
            m_afValues[0] = fLower;
            m_afValues[1] = fUpper;
            Debug.Log("Range [" + m_afValues[0] + ", " + m_afValues[1] + "]");
            if (m_ShaderName != null) m_Renderer.material.SetFloatArray(m_ShaderName, m_afValues);
            else if(localValueEvent!=null) localValueEvent(m_Id, fLower, fUpper);
        }
        public override void apply(int iState)
        {
            m_iState = iState;
            if (m_ShaderName != null) m_Renderer.material.SetInt(m_ShaderName + "S", m_iState);
            else if(localComboEvent!=null) localComboEvent(m_Id, m_iState);
        }
        public override void apply(float[] col)
        {
            m_Colour[0] = col[0];
            m_Colour[1] = col[1];
            m_Colour[2] = col[2];
            m_Colour[3] = col[3];
            if (m_ShaderName != null) m_Renderer.material.SetFloatArray(m_ShaderName + "C", m_Colour);
            else if(localColourEvent!=null) localColourEvent(m_Id, m_Colour);
        }

        public override void intialise()
        {
            if (m_ShaderName != null)
            {
                m_Renderer.material.SetFloatArray(m_ShaderName, m_afValues);
                m_Renderer.material.SetInt(m_ShaderName + "S", m_iState);
                m_Renderer.material.SetFloatArray(m_ShaderName + "C", m_Colour);
            }
            else
            {
                if (localValueEvent != null) localValueEvent(m_Id, m_afValues[0], m_afValues[1]);
                if (localColourEvent != null) localColourEvent(m_Id, m_Colour);
                if (localComboEvent != null) localComboEvent(m_Id, m_iState);
            }
        }

        public override void addItem(ref raaInitMessage m)
        {
            if (m_bUseColour)
                m.addSlider(m_DisplayName, m_Id, m_fMin, m_fMax, m_afValues[0], m_afValues[1], m_Colour, true, m_iState, m_Modes);
            else
                m.addSlider(m_DisplayName, m_Id, m_fMin, m_fMax, m_afValues[0], m_afValues[1], false, m_iState, m_Modes);
        }
    }

    class raaInterfaceItemCheckBoxBroup : raaInterfaceItem
    {
        Dictionary<uint, raaCheckBox> checkboxes = new Dictionary<uint, raaCheckBox>();
        checkBoxEvent localEvent = null;

        public raaInterfaceItemCheckBoxBroup(string displayName) : base(displayName, Types.CheckBoxGroup)
        {

        }
        public raaInterfaceItemCheckBoxBroup(string displayName, checkBoxEvent valueEvent) : base(displayName, Types.CheckBoxGroup)
        {
            localEvent = valueEvent;
        }

        public int addCheckBox(string sName, string sShader, bool bState)
        {
            int id = checkboxes.Count;
            checkboxes.Add((uint)checkboxes.Count, new raaCheckBox((uint)checkboxes.Count, bState, sName, sShader));
            return id;
        }
        public int addCheckBox(string sName, bool bState)
        {
            int id = checkboxes.Count;
            checkboxes.Add((uint)checkboxes.Count, new raaCheckBox((uint)checkboxes.Count, bState, sName));
            return id;
        }

        public override void apply(uint groupId, uint boxId, bool state)
        {
            if(checkboxes.ContainsKey(boxId) && m_Id==groupId)
            {
                checkboxes[boxId].setState(state);
                if (checkboxes[boxId].shader() != null) m_Renderer.material.SetInt(checkboxes[boxId].shader(), state ? 1 : 0);
                else if (localEvent != null) localEvent(groupId, boxId, state);
            }
        }
        public override void intialise()
        {
            foreach(raaCheckBox cb in checkboxes.Values)
            {
                if (cb.shader() != null) m_Renderer.material.SetInt(cb.shader(), cb.state() ? 1 : 0);
                else if (localEvent != null) localEvent(m_Id, cb.id(), cb.state());
            }
        }

        public override void addItem(ref raaInitMessage m)
        {
            m.addCheckBoxGroup(m_DisplayName, m_Id, checkboxes);

        }
    }

    Dictionary<uint, raaInterfaceItem> items = new Dictionary<uint, raaInterfaceItem>();

    public raaInterfaceManager()
    {
        sliderFunction = sliderEventProcessor;
        sliderIntFunction = sliderIntEventProcessor;
        rangeSliderValueFunction = rangeSliderValueEventProcessor;
        rangeSliderStateFunction = rangeSliderStateEventProcessor;
        rangeSliderComboFunction = rangeSliderComboEventProcessor;
        rangeSliderColourFunction = rangeSliderColourEventProcessor;
    }

    void sliderEventProcessor(uint id, float value)
    {
        if (items.ContainsKey(id)) items[id].apply(value);
    }

    void sliderIntEventProcessor(uint id, int value)
    {
        if (items.ContainsKey(id)) items[id].apply(value);
    }
    void rangeSliderValueEventProcessor(uint id, float lower, float upper)
    {
        if (items.ContainsKey(id)) items[id].apply(lower, upper);
    }
    void rangeSliderStateEventProcessor(uint id, bool bState)
    {
        if (items.ContainsKey(id)) items[id].apply(bState);
    }
    void rangeSliderComboEventProcessor(uint id, int iState)
    {
        if (items.ContainsKey(id)) items[id].apply(iState);
    }
    void rangeSliderColourEventProcessor(uint id, float[] col)
    {
        if (items.ContainsKey(id)) items[id].apply(col);
    }
    void checkboxEventProcessor(uint groupId, uint boxId, bool state)
    {
        if (items.ContainsKey(groupId))
        {
            items[groupId].apply(groupId, boxId, state);
        }
    }

    public void sendInitMsg(raaServer s)
    {
        if (s != null)
        {
            raaInitMessage m = new raaInitMessage();
            foreach (KeyValuePair<uint, raaInterfaceItem> i in items)
            {
                i.Value.addItem(ref m);
            }
            s.SendMessage(m);
        }
    }
    public void sendInitMsg(raaAsyncServer s)
    {
        if (s != null)
        {
            raaInitMessage m = new raaInitMessage();
            foreach (KeyValuePair<uint, raaInterfaceItem> i in items)
            {
                i.Value.addItem(ref m);
            }
            s.send(m);
        }
    }

    public void process(Renderer r, raaMessage m)
    {
        raaInterfaceItem.m_Renderer = r;
        parse(m);
    }
    public void initalise(Renderer r)
    {
        raaInterfaceItem.m_Renderer = r;
        foreach (KeyValuePair<uint, raaInterfaceItem> i in items) i.Value.intialise();
    }

    public void addSlider(string displayName, string shaderName, float fValue, float fMin, float fMax)
    {
        raaInterfaceItemSlider s = new raaInterfaceItemSlider(displayName, shaderName, fValue, fMin, fMax);
        items.Add(s.m_Id, s);
    }
    public void addSlider(string displayName, sliderEvent valueEvent, float fValue, float fMin, float fMax)
    {
        raaInterfaceItemSlider s = new raaInterfaceItemSlider(displayName, valueEvent, fValue, fMin, fMax);
        items.Add(s.m_Id, s);
    }
    public void addIntSlider(string displayName, string shaderName, int iValue, int iMin, int iMax)
    {
        raaInterfaceItemSliderInt s = new raaInterfaceItemSliderInt(displayName, shaderName, iValue, iMin, iMax);
        items.Add(s.m_Id, s);
    }
    public void addIntSlider(string displayName, sliderIntEvent valueEvent, int iValue, int iMin, int iMax)
    {
        raaInterfaceItemSliderInt s = new raaInterfaceItemSliderInt(displayName, valueEvent, iValue, iMin, iMax);
        items.Add(s.m_Id, s);
    }
    public void addRangeSlider(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, float[] colour, bool bState, bool bUseColour)
    {
        raaInterfaceItemRangeSlider s = new raaInterfaceItemRangeSlider(displayName, shaderName, fLower, fUpper, fMin, fMax, bState, bUseColour, colour);
        items.Add(s.m_Id, s);
    }
    public void addRangeSlider(string displayName, rangeSliderValueEvent valueEvent, rangeSliderStateEvent stateEvent, rangeSliderColourEvent colourEvent, float fLower, float fUpper, float fMin, float fMax, float[] colour, bool bState, bool bUseColour)
    {
        raaInterfaceItemRangeSlider s = new raaInterfaceItemRangeSlider(displayName, valueEvent, stateEvent, colourEvent, fLower, fUpper, fMin, fMax, bState, bUseColour, colour);
        items.Add(s.m_Id, s);
    }
    public void addRangeSlider(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, bool bState)
    {
        raaInterfaceItemRangeSlider s = new raaInterfaceItemRangeSlider(displayName, shaderName, fLower, fUpper, fMin, fMax, bState);
        items.Add(s.m_Id, s);
    }
    public void addRangeSlider(string displayName, rangeSliderValueEvent valueEvent, rangeSliderStateEvent stateEvent, float fLower, float fUpper, float fMin, float fMax, bool bState)
    {
        raaInterfaceItemRangeSlider s = new raaInterfaceItemRangeSlider(displayName, valueEvent, stateEvent, fLower, fUpper, fMin, fMax, bState);
        items.Add(s.m_Id, s);
    }
    public void addRangeSliderCombo(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, float[] colour, bool bUseColour, int state, string[] modes)
    {
        raaInterfaceItemRangeSliderCombo s = new raaInterfaceItemRangeSliderCombo(displayName, shaderName, fLower, fUpper, fMin, fMax, state, modes, bUseColour, colour);
        items.Add(s.m_Id, s);
    }
    public void addRangeSliderCombo(string displayName, rangeSliderValueEvent valueEvent, rangeSliderComboEvent comboEvent, rangeSliderColourEvent colourEvent, float fLower, float fUpper, float fMin, float fMax, float[] colour, bool bUseColour, int state, string[] modes)
    {
        raaInterfaceItemRangeSliderCombo s = new raaInterfaceItemRangeSliderCombo(displayName, valueEvent, comboEvent, colourEvent, fLower, fUpper, fMin, fMax, state, modes, bUseColour, colour);
        items.Add(s.m_Id, s);
    }
    public void addRangeSliderCombo(string displayName, string shaderName, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes)
    {
        raaInterfaceItemRangeSliderCombo s = new raaInterfaceItemRangeSliderCombo(displayName, shaderName, fLower, fUpper, fMin, fMax, iState, modes);
        items.Add(s.m_Id, s);
    }
    public void addRangeSliderCombo(string displayName, rangeSliderValueEvent valueEvent, rangeSliderComboEvent comboEvent, float fLower, float fUpper, float fMin, float fMax, int iState, string[] modes)
    {
        raaInterfaceItemRangeSliderCombo s = new raaInterfaceItemRangeSliderCombo(displayName, valueEvent, comboEvent, fLower, fUpper, fMin, fMax, iState, modes);
        items.Add(s.m_Id, s);
    }

    public uint addCheckboxGroup(string displayName)
    {
        raaInterfaceItemCheckBoxBroup cbg = new raaInterfaceItemCheckBoxBroup(displayName);
        items.Add(cbg.m_Id, cbg);
        return cbg.m_Id;
    }
    public uint addCheckboxGroup(string displayName, checkBoxEvent valueEvent)
    {
        raaInterfaceItemCheckBoxBroup cbg = new raaInterfaceItemCheckBoxBroup(displayName, valueEvent);
        items.Add(cbg.m_Id, cbg);
        return cbg.m_Id;
    }

    public int addCheckBox(string name, uint group, string shaderName, bool state)
    {
        if (items.ContainsKey(group))
        {
            return ((raaInterfaceItemCheckBoxBroup)items[group]).addCheckBox(name, shaderName, state);
        }
        return -1;
    }
    public int addCheckBox(string name, uint group, bool state)
    {
        if (items.ContainsKey(group))
        {
            return ((raaInterfaceItemCheckBoxBroup)items[group]).addCheckBox(name, state);
        }
        return -1;
    }
}