﻿using statsd.net.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace statsd.net.Backends
{
  public class GraphiteBackend : IBackend
  {
    private UdpClient _client;
    private Task _completionTask;
    private bool _isActive;
    
    public GraphiteBackend(string host, int port)
    {
      var ipAddress = Utility.HostToIPv4Address(host);
      _client = new UdpClient();
      _client.Connect(ipAddress, port);
      _completionTask = new Task(() => { _isActive = false; });
    }

    public bool IsActive
    {
      get { return _isActive; }
    }

    public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, GraphiteLine messageValue, ISourceBlock<GraphiteLine> source, bool consumeToAccept)
    {
      byte[] data = Encoding.ASCII.GetBytes(messageValue.ToString());
      _client.Send(data, data.Length);
      return DataflowMessageStatus.Accepted;
    }

    public void Complete()
    {
      _completionTask.Start();
    }

    public Task Completion
    {
      get { return _completionTask; }
    }

    public void Fault(Exception exception)
    {
      throw new NotImplementedException();
    }
  }
}