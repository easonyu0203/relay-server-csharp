using System.Net.Sockets;
using relay_server.Payload;

namespace relay_server;

public class Hotel : Singleton<Hotel>
{
    public Dictionary<Int32, Room> Rooms = new Dictionary<int, Room>();
    public Dictionary<RelayUser, HashSet<Room>> UserRooms = new Dictionary<RelayUser, HashSet<Room>>();

    public void JoinRoom(int roomId, RelayUser relayUser)
    {
        if (Rooms.ContainsKey(roomId) == false)
        {
            Rooms.Add(roomId, new Room());
        }

        if (UserRooms.ContainsKey(relayUser) == false)
        {
            UserRooms.Add(relayUser, new HashSet<Room>());
        }

        Rooms[roomId].AddUser(relayUser);
        UserRooms[relayUser].Add(Rooms[roomId]);
        relayUser.OnDisconnect += () =>
        {
            Rooms[roomId].RemoveUser(relayUser);
            UserRooms.Remove(relayUser);
        };
    }

    public bool LeaveRoom(int roomId, RelayUser relayUser)
    {
        if(!UserRooms.ContainsKey(relayUser)) return false;
        Rooms[roomId].RemoveUser(relayUser);
        UserRooms[relayUser].Remove(Rooms[roomId]);
        return true;
    }
}

public class Room
{
    private HashSet<RelayUser> _users = new HashSet<RelayUser>();

    public void AddUser(RelayUser relayUser)
    {
        _users.Add(relayUser);
    }

    public void RemoveUser(RelayUser relayUser)
    {
        _users.Remove(relayUser);
    }

    public void RelayPayload(BasePayload relayBasePayload, RelayUser relayUser)
    {
        foreach (RelayUser other in _users)
        {
            if(relayUser == other) continue;
            other.SendPayload(relayBasePayload);
        }
    }
}