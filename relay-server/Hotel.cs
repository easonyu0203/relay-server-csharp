using System.Net.Sockets;

namespace relay_server;

public class Hotel : Singleton<Hotel>
{
    public Dictionary<Int32, Room> Rooms = new Dictionary<int, Room>();
    public Dictionary<User, HashSet<Room>> UserRooms = new Dictionary<User, HashSet<Room>>();

    public void JoinRoom(int roomId, User user)
    {
        if (Rooms.ContainsKey(roomId) == false)
        {
            Rooms.Add(roomId, new Room());
        }

        if (UserRooms.ContainsKey(user) == false)
        {
            UserRooms.Add(user, new HashSet<Room>());
        }

        Rooms[roomId].AddUser(user);
        UserRooms[user].Add(Rooms[roomId]);
        user.OnDisconnect += () =>
        {
            Rooms[roomId].RemoveUser(user);
            UserRooms.Remove(user);
        };
    }

    public bool LeaveRoom(int roomId, User user)
    {
        if(!UserRooms.ContainsKey(user)) return false;
        Rooms[roomId].RemoveUser(user);
        UserRooms[user].Remove(Rooms[roomId]);
        return true;
    }
}

public class Room
{
    private HashSet<User> _users = new HashSet<User>();

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public void RemoveUser(User user)
    {
        _users.Remove(user);
    }

    public void RelayPayload(Payload relayPayload, User user)
    {
        foreach (User other in _users)
        {
            if(user == other) continue;
            other.SendPayload(relayPayload);
        }
    }
}