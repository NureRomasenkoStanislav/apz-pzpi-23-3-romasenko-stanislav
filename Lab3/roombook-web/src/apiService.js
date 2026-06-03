const BASE_URL = "https://localhost:7242"; 

export const apiService = {
  async login(email, password) {
    const response = await fetch(`${BASE_URL}/api/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ 
        email: email, // camelCase відповідно до стандартів JSON-серіалізації .NET
        password: password 
      }),
    });

    if (!response.ok) {
      throw new Error("Неправильні облікові дані");
    }
    return await response.json();
  },

  async getRooms(token) {
    const response = await fetch(`${BASE_URL}/api/rooms`, {
      headers: { "Authorization": `Bearer ${token}` },
    });
    if (!response.ok) throw new Error("Помилка завантаження кімнат");
    return await response.json();
  },

  async createBooking(token, roomId, chosenTime) {
    const startTimeIso = new Date(chosenTime).toISOString();
    
    const endTimeIso = new Date(new Date(chosenTime).getTime() + 60 * 60 * 1000).toISOString();

    const response = await fetch(`${BASE_URL}/api/bookings`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({
        userId: 1, 
        roomId: parseInt(roomId),
        startTime: startTimeIso, 
        endTime: endTimeIso,    
        purpose: "Бронювання через Web-інтерфейс"
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || "Помилка бронювання на сервері");
    }
    return await response.json();
  },

  async createRoom(token, roomData) {
    const response = await fetch(`${BASE_URL}/api/rooms`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({ 
        name: roomData.name, 
        capacity: parseInt(roomData.capacity),
        description: "Створено з веб-інтерфейсу",
        workingHoursStart: "09:00:00", 
        workingHoursEnd: "18:00:00",
        isArchived: false
      }),
    });

    if (!response.ok) throw new Error("Помилка створення кімнати");
    return await response.json();
  },

  async updateRoom(token, id, roomData) {
    const response = await fetch(`${BASE_URL}/api/rooms/${parseInt(id)}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({ 
        roomId: parseInt(id),
        name: roomData.name, 
        capacity: parseInt(roomData.capacity),
        description: "Оновлено з веб-інтерфейсу",
        workingHoursStart: "09:00:00",
        workingHoursEnd: "18:00:00",
        isArchived: false
      }),
    });

    if (!response.ok) throw new Error("Помилка оновлення кімнати");
    if (response.status === 204) return { status: "success" };
    return response.text().then(text => text ? JSON.parse(text) : {});
  },

  async deleteRoom(token, id) {
    const numericId = parseInt(id);
    if (isNaN(numericId)) throw new Error("Invalid ID");

    const response = await fetch(`${BASE_URL}/api/rooms/${numericId}`, {
      method: "DELETE",
      headers: { "Authorization": `Bearer ${token}` },
    });

    if (!response.ok) {
      throw new Error("Сервер повернув помилку при видаленні");
    }
  }
};