/* TelnetParser
 * (c) Stefan Licht, 2009
 * 
 * This class parse a bunch of bytes and for each recognized TelnetSymbol an event 
 * will be invoked an could be handled from the caller
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using sones.Networking.Telnet.Events;

namespace sones.Networking.Telnet
{
    public class TelnetParser
    {

        #region Events

        public event TelnetCommandEventHandler OnCommandReceived;

        /// <summary>
        /// The client request this option, set the Accepted args to send will or wont
        /// </summary>
        public event TelnetOptionEventHandler  OnDoOptionReceived;

        /// <summary>
        /// The client unrequest this option, set the Accepted args to send will or wont
        /// </summary>
        public event TelnetOptionEventHandler  OnDontOptionReceived;
        
        /// <summary>
        /// The client will acceppt this option
        /// </summary>
        public event TelnetOptionEventHandler  OnWillOptionReceived;

        /// <summary>
        /// The client will NOT acceppt this option
        /// </summary>
        public event TelnetOptionEventHandler  OnWontOptionReceived;
        
        /// <summary>
        /// New data recieved
        /// </summary>
        public event TelnetDataEventHandler    OnDataReceived;

        /// <summary>
        /// New data arrived which should be send
        /// </summary>
        public event TelnetDataEventHandler    OnDataToSendArrived;

        /// <summary>
        /// A Subnegotiation data arrived
        /// </summary>
        public event TelnetSubnegotiationEventHandler OnSubnegotiationRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event TelnetKeyEventHandler    OnKeyReceived;

        #endregion

        BitArray _LocalOptions;
        BitArray _RemoteOptions = new BitArray(256, false);

        public TelnetParser()
        {
            _LocalOptions = new BitArray(256, false);
            _RemoteOptions = new BitArray(256, false);
        }

        public Boolean Parse(Byte[] myBytes)
        {

            //List<TelnetSymbol> resolvedSymbols = new List<TelnetSymbol>();
            TelnetSymbolTypes lastByteType = TelnetSymbolTypes.Unknown;
            List<Byte> dataBytes = new List<byte>();
            List<Byte> subnegotiationBytes = new List<byte>();

            for (Int32 i=0; i < myBytes.Length; i++)
            {

                //As required by the Telnet protocol, any occurrence of 255 in the subnegotiation must be doubled to distinguish it from the IAC character (which has a value of 255)
                if (myBytes[i] == (Byte)TelnetCommands.Iac || lastByteType == TelnetSymbolTypes.Subnegotiation)
                {

                    if (lastByteType == TelnetSymbolTypes.Data)
                    {
                        HandleData(dataBytes.ToArray());
                        HandleKeys(dataBytes.ToArray());
                        dataBytes.Clear();
                    }

                    #region Read the next command or subnegotiation data

                    TelnetCommands command;
                    if (lastByteType == TelnetSymbolTypes.Subnegotiation)
                        command = (TelnetCommands)myBytes[i];
                    else
                        command = (TelnetCommands)myBytes[++i];

                    if (lastByteType == TelnetSymbolTypes.Subnegotiation)
                    {
                        if (command != TelnetCommands.Iac || myBytes[i + 1] != (Byte)TelnetCommands.SubnegotiationEnd)
                        {
                            Console.Write(myBytes[i] + ", ");
                            subnegotiationBytes.Add(myBytes[i]);

                            //As required by the Telnet protocol, any occurrence of 255 in the subnegotiation must be doubled to distinguish it from the IAC character (which has a value of 255)
                            if (command == TelnetCommands.Iac && i + 1 < myBytes.Length && myBytes[i + 1] == (Byte)TelnetCommands.Iac) 
                                i++;

                            continue;
                        }
                        else
                        {
                            command = (TelnetCommands)myBytes[++i];
                        }
                    }

                    #endregion

                    switch (command)
                    {

                        #region Subnegotiation

                        case TelnetCommands.Subnegotiation:
                            //throw new NotImplementedException("TelnetCommands.Subnegotiation");
                            Console.WriteLine("TelnetCommands.Subnegotiation");
                            subnegotiationBytes.Clear();
                            lastByteType = TelnetSymbolTypes.Subnegotiation;
                            break;
                        case TelnetCommands.SubnegotiationEnd:
                            //throw new NotImplementedException("TelnetCommands.SubnegotiationEnd");
                            Console.WriteLine("TelnetCommands.SubnegotiationEnd");
                            HandleSubnegotiation(subnegotiationBytes.ToArray());
                            lastByteType = TelnetSymbolTypes.Unknown;
                            break;

                        #endregion

                        #region Simple commands without option

                        case TelnetCommands.NoOperation:
                        case TelnetCommands.DataMark:
                        case TelnetCommands.Break:
                        case TelnetCommands.InterruptProcess:
                        case TelnetCommands.AbortOutput:
                        case TelnetCommands.AreYouThere:
                        case TelnetCommands.EraseCharacter:
                        case TelnetCommands.EraseLine:
                        case TelnetCommands.GoAhead:
                            //resolvedSymbols.Add(new TelnetSymbol(command));
                            lastByteType = TelnetSymbolTypes.Command;
                            HandleTelnetCommand(new TelnetSymbol(command));
                            break;

                        #endregion

                        #region Option commands

                        case TelnetCommands.Will:
                        case TelnetCommands.Wont:
                        case TelnetCommands.Do:
                        case TelnetCommands.Dont:
                            TelnetOptions option = (TelnetOptions)myBytes[++i];
                            //resolvedSymbols.Add(new TelnetSymbol(command, option));
                            lastByteType = TelnetSymbolTypes.Command;
                            HandleTelnetOption(new TelnetSymbol(command, option));
                            break;

                        #endregion

                        default:
                            throw new NotSupportedException("Not supported command: " + command);
                    }
                }
                else
                {
                    lastByteType = TelnetSymbolTypes.Data;
                    //if (myBytes[i] != (Byte)ASCIIControlCodes.CR && myBytes[i] != (Byte)ASCIIControlCodes.LF)
                    dataBytes.Add(myBytes[i]);
                }
            }

            if (lastByteType == TelnetSymbolTypes.Data && dataBytes.Count > 0)
            {
                HandleData(dataBytes.ToArray());
                HandleKeys(dataBytes.ToArray());
            }

            return true;

        }

        #region Handle Events

        private void HandleSubnegotiation(Byte[] mySubnegotiationBytes)
        {
            if (OnSubnegotiationRecieved != null)
            {
                TelnetSubnegotiationEventArgs args = new TelnetSubnegotiationEventArgs();
                args.TelnetOption = (TelnetOptions)mySubnegotiationBytes[0];
                args.ContentData = new Byte[mySubnegotiationBytes.Length - 1]; // remove first (Option)
                Array.Copy(mySubnegotiationBytes, 1, args.ContentData, 0, args.ContentData.Length);

                OnSubnegotiationRecieved(this, args);
            }
        }

        private void HandleTelnetCommand(TelnetSymbol myTelnetSymbol)
        {
            if (OnCommandReceived != null)
            {
                TelnetCommandEventArgs args = new TelnetCommandEventArgs();
                args.TelnetSymbol = myTelnetSymbol;
                OnCommandReceived(this, args);
            }
        }

        private void HandleTelnetOption(TelnetSymbol myTelnetSymbol)
        {
            switch (myTelnetSymbol.Command)
            {
                case TelnetCommands.Will:
                    HandleTelnetWillOption(myTelnetSymbol);
                    break;
                case TelnetCommands.Wont:
                    HandleTelnetWontOption(myTelnetSymbol);
                    break;
                case TelnetCommands.Do:
                    HandleTelnetDoOption(myTelnetSymbol);
                    break;
                case TelnetCommands.Dont:
                    HandleTelnetDontOption(myTelnetSymbol);
                    break;
            }
        }

        private void HandleTelnetWillOption(TelnetSymbol myTelnetSymbol)
        {

            if (OnWillOptionReceived != null)
            {
                TelnetOptionEventArgs args = new TelnetOptionEventArgs();
                args.TelnetSymbol = myTelnetSymbol;
                OnWillOptionReceived(this, args);

                // If the remote option changed than set this option and send a Do or Dont depending on accepted
                if (args.Accepted != _RemoteOptions[(Int32)myTelnetSymbol.Option])
                {
                    _RemoteOptions[(Int32)myTelnetSymbol.Option] = args.Accepted;
                    if (args.Accepted)
                        SendDoOption(myTelnetSymbol.Option);
                    else
                        SendDontOption(myTelnetSymbol.Option);
                }
            }

        }

        private void HandleTelnetWontOption(TelnetSymbol myTelnetSymbol)
        {

            if (OnWontOptionReceived != null)
            {
                TelnetOptionEventArgs args = new TelnetOptionEventArgs();
                args.TelnetSymbol = myTelnetSymbol;
                OnWontOptionReceived(this, args);

                // Set the remote option to false and send dont
                if (_RemoteOptions[(Int32)myTelnetSymbol.Option])
                {
                    SendDontOption(myTelnetSymbol.Option);
                    _RemoteOptions[(Int32)myTelnetSymbol.Option] = false;
                }
                // The option is already set to false, so do nothing
                else
                {
                }
            }

        }
        
        private void HandleTelnetDoOption(TelnetSymbol myTelnetSymbol)
        {

            if (OnDoOptionReceived != null)
            {
                TelnetOptionEventArgs args = new TelnetOptionEventArgs();
                args.TelnetSymbol = myTelnetSymbol;
                OnDoOptionReceived(this, args);

                if (args.Accepted)
                {
                    // only if we did not set this option already
                    if (!_LocalOptions[(Int32)myTelnetSymbol.Option])
                    {
                        _LocalOptions[(Int32)myTelnetSymbol.Option] = true;
                        SendWillOption(myTelnetSymbol.Option);
                    }
                }
                else
                {
                    _LocalOptions[(Int32)myTelnetSymbol.Option] = false;
                    SendWontOption(myTelnetSymbol.Option);
                }

            }

        }
        
        private void HandleTelnetDontOption(TelnetSymbol myTelnetSymbol)
        {

            if (OnDontOptionReceived != null)
            {
                TelnetOptionEventArgs args = new TelnetOptionEventArgs();
                args.TelnetSymbol = myTelnetSymbol;
                OnDontOptionReceived(this, args);

                // we accepted the dont of this option
                if (args.Accepted)
                {
                    _LocalOptions[(Int32)myTelnetSymbol.Option] = false;
                    SendWontOption(myTelnetSymbol.Option);
                }
                // if we do not accept the dont we could try to send a SendWillOption
                else
                {
                }

            }

        }

        private void HandleData(Byte[] myData)
        {
            if (OnDataReceived != null)
            {
                TelnetDataEventArgs args = new TelnetDataEventArgs();
                args.Data = myData;

                OnDataReceived(this, args);
            }

        }

        private void HandleKeys(Byte[] myData)
        {
            if (OnKeyReceived != null)
            {

                Byte[] bytesToSend = null;
                //Console.Write("[TelnetParser_OnDataReceived] DataLength: " + myEventArgs.Data.Length + "Key: ");
                Int32 i=0;
                while (i < myData.Length)
                {
                    ConsoleKeyInfo? keyInfo = null;
                    Byte b = myData[i++];
                    //Write((char)b + " = " + b.ToString() + ", ");

                    #region Parse bytes and create ConsoleKeyInfo

                    // Special key, e.g.: ! " # $ %
                    if ((b >= 32 && b <= 47) || (b >= 58 && b <= 64) || (b >= 91 && b <= 96) || (b >= 123 && b <= 126))
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, (ConsoleKey)0, false, false, false);
                        bytesToSend = new Byte[1];
                        bytesToSend[0] = b;
                    }

                    // numbers
                    else if (b >= 48 && b <= 57)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, (ConsoleKey)b, false, false, false);
                        bytesToSend = new Byte[1];
                        bytesToSend[0] = b;
                    }

                    // char a-z
                    else if (b >= 65 && b <= 90)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, (ConsoleKey)b, false, false, false);
                        bytesToSend = new Byte[1];
                        bytesToSend[0] = b;
                    }

                    // char A-Z
                    else if (b >= 97 && b <= 122)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, (ConsoleKey)(b - 32), true, false, false);
                        bytesToSend = new Byte[1];
                        bytesToSend[0] = b;
                    }

                    // Arrows
                    else if (b == 27 && (i + 1 < myData.Length) && myData[i] == 91 && myData[i + 1] >= 65 && myData[i + 1] <= 68)
                    {
                        b = myData[i + 1];
                        switch (b)
                        {
                            case 65:
                                keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.UpArrow, false, false, false);
                                break;
                            case 66:
                                keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.DownArrow, false, false, false);
                                break;
                            case 67:
                                keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.RightArrow, false, false, false);
                                bytesToSend = new Byte[3];
                                Array.Copy(myData, i - 1, bytesToSend, 0, 3);
                                break;
                            case 68:
                                keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.LeftArrow, false, false, false);
                                bytesToSend = new Byte[3];
                                Array.Copy(myData, i - 1, bytesToSend, 0, 3);
                                break;
                        }
                        i += 2;
                    }

                    // TAB
                    else if (b == 9)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, ConsoleKey.Tab, false, false, false);
                        bytesToSend = new Byte[3] { 27, 91, 50 };
                    }

                    // ESC
                    else if (b == 27)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, ConsoleKey.Escape, false, false, false);
                        bytesToSend = new Byte[3] { 27, 91, 50 };
                    }

                    // Delete (Entf)
                    else if (b == 127)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, ConsoleKey.Delete, false, false, false);
                        bytesToSend = new Byte[1] { b };
                    }

                    // Backspace
                    else if (b == 8)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, ConsoleKey.Backspace, false, false, false);
                        bytesToSend = new Byte[1] { b };
                    }

                    // Enter
                    else if (b == (Byte)ASCIIControlCodes.CR)
                    {
                        keyInfo = new ConsoleKeyInfo((char)b, ConsoleKey.Enter, false, false, false);
                        if (myData[i] == 0) i++;
                    }

                    #endregion

                    if (keyInfo.HasValue)
                    {

                        TelnetKeyEventArgs args = new TelnetKeyEventArgs();
                        args.KeyInfo = keyInfo.Value;
                        OnKeyReceived(this, args);
                    
                    }
                }

            }

        }

        private void HandleDataToSend(Byte[] myData)
        {
            if (OnDataToSendArrived != null)
            {
                TelnetDataEventArgs args = new TelnetDataEventArgs();
                args.Data = myData;
                OnDataToSendArrived(this, args);
            }
        }

        #endregion

        #region Set/Unset Options

        public void SetWillOption(TelnetOptions myOption)
        {
            if (_LocalOptions[(Int32)myOption] == _RemoteOptions[(Int32)myOption])
            {
                SendWillOption(myOption);
            }
        }

        public void SetWontOption(TelnetOptions myOption)
        {
            if (_LocalOptions[(Int32)myOption] == _RemoteOptions[(Int32)myOption])
            {
                SendWontOption(myOption);
            }
        }

        public void SetOption(TelnetOptions myOption)
        {
            if (_LocalOptions[(Int32)myOption] == _RemoteOptions[(Int32)myOption])
            {
                SendDoOption(myOption);
            }
        }

        public void UnsetOption(TelnetOptions myOption)
        {
            if (_LocalOptions[(Int32)myOption] == _RemoteOptions[(Int32)myOption])
            {
                SendDontOption(myOption);
            }
        }

        #endregion

        #region Send...Option

        /// <summary>
        /// If we are willing to accept the requested option
        /// </summary>
        /// <param name="option"></param>
        private void SendWillOption(TelnetOptions myOption)
        {
            SendOption(TelnetCommands.Will, myOption);
        }

        /// <summary>
        /// If we won't accept  the requested option
        /// </summary>
        /// <param name="option"></param>
        private void SendWontOption(TelnetOptions myOption)
        {
            SendOption(TelnetCommands.Wont, myOption);
        }

        /// <summary>
        /// Send a do option request
        /// </summary>
        /// <param name="option"></param>
        private void SendDoOption(TelnetOptions myOption)
        {
            SendOption(TelnetCommands.Do, myOption);
        }

        /// <summary>
        /// Send a dont option request
        /// </summary>
        /// <param name="myOption"></param>
        private void SendDontOption(TelnetOptions myOption)
        {
            SendOption(TelnetCommands.Dont, myOption);
        }

        private void SendOption(TelnetCommands myCommand, TelnetOptions myOption)
        {
            Byte[] data = new Byte[] { (Byte)TelnetCommands.Iac, (Byte)myCommand, (Byte)myOption };
            HandleDataToSend(data);
        }

        #endregion

    }
}
