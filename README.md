
# c# Relay Server

  

Duration: October 11, 2022 → October 13, 2022

Status: complete

Type: C#

  

# Introduction

  

C# implementation of relay server, this relay server have following attribute.

  

- Low latency

- Low bandwidth

- Support multiple room

  

# Project Motivation

  

I want to make a client-to-client real-time game, and want to use a ****relay server**** to let clients communicate. But I don’t find any online relay server implementation meet my need. Hence I write this project.

  

# Get Start!

  

1. clone and build both relay-server & relay-client solutions

2. run `dotnet relay.dll -h localhost -p 3000` to start relay server on localhost port 3000

3. run `dotnet client.dll -h localhost -p 3000` to start client (run at least 2 client)

4. run `JOIN 1` to join room with id 1

5. start another client and run `JOIN 1` to join room with id 1

6. run `MSG “hello world”` to send “hello world” to the room

7. you should see “hello world” in all clients exclude sender

  

# Document

  

## Connection

  

relay server are connected by tcp

  

## Protocol

  

- Payload is transmit in bytes, and all payload look like this

- 1 byte: id of payload_type (type int32)

- 4 byte: payload_size (type int32)

- body_size byte: body

```jsx

Payload{

int32 payload_type;

int32 body_size;

byte[payload_size] body;

int32 end_flag = -11;

}

```

- Payload type

- Payload description

- Client & Server side

- MSG

| Description | to send message between client and server |

| --- | --- |

| payload_type | 0 |

| body_size | size of body in byte |

| body | contain the message want to send |

- Client side

- JOIN:

| Description | Client request join room |

| --- | --- |

| payload_type | 1 |

| body_size | size of body in byte |

| body | contain room id (id type int32) |

- Leave:

| Description | Client request leave room |

| --- | --- |

| payload_type | 2 |

| body_size | size of body in byte |

| body | contain room id (id type int32) |

- PING:

| Description | client ping server |

| --- | --- |

| payload_type | 3 |

| body_size | None |

| body | None |

- CLOSE:

| Description | client request server to close connection |

| --- | --- |

| payload_type | 4 |

| body_size | None |

| body | None |

- Server side

- PING:

| Description | server response to client PING |

| --- | --- |

| payload_type | 3 |

| body_size | None |

| body | None |

- STATUS

| Description | status code to indicate status |

| --- | --- |

| payload_type | 5 |

| body_size | 4 byte |

| body | status_code (type int) 200=success, 404=server can’t handle command |

- Payload id

```jsx

MSG: 0

JOIN: 1

LEAVE: 2

PING: 3

CLOSE: 4

STATUS: 5

```

# Project Asset

[https://github.com/easonyu0203/relay-server-csharp](https://github.com/easonyu0203/relay-server-csharp)
