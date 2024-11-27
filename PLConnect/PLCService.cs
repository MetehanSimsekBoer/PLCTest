using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;
using Sharp7;


namespace PLConnect
{
    class PLCService
    {
        ModbusClient modbusClient = new ModbusClient();
        private S7Client plcS7Client = new S7Client();
        public PLCService(string ipAddres,int port)
        {
            modbusClient = new ModbusClient(ipAddres,port);
            plcS7Client = new S7Client(ipAddres);   
        }

        public void Connect()
        {
            modbusClient.Connect();
            Console.WriteLine("Başarı Ile bağlandı...");
            

        }
        public bool ConnectSiementPLC(string ipAddress, int rack, int slot)
        {
           
            int result = plcS7Client.ConnectTo(ipAddress, 0, 1);
            return result == 0; 
        }
        public void DisConnectSiemensPLC()
        {
            var result = plcS7Client.Disconnect();
        }
        public void Disconnect()
        {
            modbusClient.Disconnect();
        }

        public void WriteCoil(int address, bool value)
        {
            modbusClient.WriteSingleCoil(address, value);
            Console.WriteLine($"Coil adresine yazıldı: Adres={address}, Değer={value}");


        }

        public void SendCommand()
        {
            #region startcommand
            byte[] buffer = new byte[1];
            buffer[0] = 0x01;
            int dbNumber = 1;
            int startByte = 0;
            int bitIndex = 1;

           



            byte[] readBuffer = new byte[1];
            //DbRead Read işlem göre Area ve WordLeni otomatik gerçekleştirir.
            var IsRead = plcS7Client.DBRead(dbNumber, startByte, 1, readBuffer);

            readBuffer[0] = (byte)(readBuffer[0] | (1 << bitIndex));

            //Aynı şekilde DBWrite Write işlem için Area ve Wordleni otomatik gerçekleştirir
            int writeResult = plcS7Client.DBWrite(dbNumber, startByte, 1, readBuffer);
            if (writeResult == 0)
            {
                Console.WriteLine("Start komutu başarıyla gönderildi");
            }
            else
            {
                Console.WriteLine("Komut gönderilemedi");
            }
            #endregion
            #region stopcommand

            #endregion

        }


        public int[] ReadRegisters(int startAddress, int quantity)
        {
            return modbusClient.ReadHoldingRegisters(startAddress, quantity);
        }

        public byte[] ReadArea( int Amount, int WordLen)
        {
            // S7 alanı için parametreler //Start Komut 0.1 Adresi için...
            int area = 0x81;  
            int dbNumber = 0; 
            int byteAddress = 0; 
            int bitAddress = 1;
            int start = 0;

            byte[] buffer = new byte[Amount * WordLen];  // Okunacak alan kadar buffer
            int result = plcS7Client.ReadArea(area, dbNumber, start, Amount, WordLen, buffer);
            
            if (result == 0)
            {
                bool startCommand = buffer[0] != 0; // Okunan değeri boolean olarak al

                if (startCommand)
                {
                    Console.WriteLine("Start komutu aktif.");
                }
                else
                {
                    Console.WriteLine("Start komutu pasif.");
                }
            }
            else
            {
                Console.WriteLine("Veri okunurken hata oluştu.");
            }
            return buffer;
        }
    }
}

//**Area table**

//S7Consts.S7AreaPE 0x81

//S7Consts.S7AreaPA 0x82

//S7Consts.S7AreaMK 0x83

//S7Consts.S7AreaDB 0x84

//S7Consts.S7AreaCT 0x1C


//S7Consts.S7AreaTM 0x1D

//**WordLen table**

//S7WLBit 0x01 Bit (inside a word)

//S7WLByte 0x02 Byte (8 bit)

//S7WLWord 0x04 Word (16 bit)

//S7WLDWord 0x06 Double Word (32 bit)

//S7WLReal 0x08 Real (32 bit float)

//S7WLCounter 0x1C Counter (16 bit)

//S7WLTimer 0x1D Timer (16 bit)


