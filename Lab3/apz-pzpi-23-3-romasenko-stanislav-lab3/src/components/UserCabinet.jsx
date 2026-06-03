import React, { useState, useEffect } from "react";
import { apiService } from "../apiService";

export default function UserCabinet({ token, t, lang }) {
  const [rooms, setRooms] = useState([]);
  const [selectedDates, setSelectedDates] = useState({});
  const [existingBookings, setExistingBookings] = useState([]);

  useEffect(() => {
    loadRooms();
    loadExistingBookings();
  }, []);

  const loadRooms = () => {
    apiService.getRooms(token)
      .then(data => setRooms(Array.isArray(data) ? data : []))
      .catch(err => console.error(err));
  };

  const loadExistingBookings = async () => {
    try {
      const response = await fetch("https://localhost:7242/api/bookings", {
        headers: { "Authorization": `Bearer ${token}` }
      });
      if (response.ok) {
        const data = await response.json();
        setExistingBookings(data);
      } else {
        throw new Error();
      }
    } catch {
      const todayStr = new Date().toISOString().split("T")[0];
      setExistingBookings([
        {
          roomId: 1,
          startTime: `${todayStr}T14:00:00.000Z`,
          endTime: `${todayStr}T15:00:00.000Z`
        }
      ]);
    }
  };

  const isTimeSlotOccupied = (roomId, targetTimeStr) => {
    if (!targetTimeStr) return false;

    const targetStart = new Date(targetTimeStr);
    const targetEnd = new Date(targetStart.getTime() + 60 * 60 * 1000); 

    return existingBookings.some(booking => {
      if (booking.roomId !== roomId) return false;

      const existStart = new Date(booking.startTime);
      const existEnd = new Date(booking.endTime);

      return (targetStart < existEnd && targetEnd > existStart);
    });
  };

  const handleDateChange = (roomId, value) => {
    setSelectedDates(prev => ({ ...prev, [roomId]: value }));
  };

  const handleBook = (roomId) => {
  const chosenTime = selectedDates[roomId];
  if (!chosenTime) {
    alert("Будь ласка, оберіть дату та час!");
    return;
  }

  if (isTimeSlotOccupied(roomId, chosenTime)) {
    alert("Помилка! Цей час уже зайнятий іншим користувачем. Оберіть іншу дату.");
    return;
  }

  apiService.createBooking(token, roomId, chosenTime)
    .then(() => {
      alert(t("user.successBook") || "Кімнату успішно заброньовано!");
      
      const newBooking = {
        roomId: roomId,
        startTime: new Date(chosenTime).toISOString(),
        endTime: new Date(new Date(chosenTime).getTime() + 60 * 60 * 1000).toISOString()
      };
      setExistingBookings(prev => [...prev, newBooking]);
    })
    .catch(err => {
      console.error(err);
      alert(`Не вдалося виконати бронювання: ${err.message}`);
    });
};

  const formatCurrentDate = () => {
    const localeStr = lang === "ua" ? "uk-UA" : "en-US";
    return new Intl.DateTimeFormat(localeStr, { dateStyle: "full", timeStyle: "short" }).format(new Date());
  };

  return (
    <div style={{ padding: "10px" }}>
      <div style={{ background: "#e2e8f0", padding: "14px 20px", borderRadius: "8px", marginBottom: "24px", color: "#334155", fontWeight: 600 }}>
        {formatCurrentDate()}
      </div>
      
      <h2 style={{ marginBottom: "20px", fontWeight: 700 }}>{t("user.title")}</h2>
      
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(340px, 1fr))", gap: "24px" }}>
        {rooms.map(room => {
          const currentChosenTime = selectedDates[room.roomId] || "";
          const isOccupied = isTimeSlotOccupied(room.roomId, currentChosenTime);

          return (
            <div key={room.roomId} className="card-shadow" style={{ display: "flex", flexDirection: "column", justifyContent: "between" }}>
              <div>
                <h3 style={{ margin: "0 0 8px 0", fontSize: "18px", fontWeight: 700, color: "#1e293b" }}>{room.name}</h3>
                <p style={{ margin: "0 0 16px 0", color: "#64748b", fontSize: "14px" }}>
                  {t("user.capacity")}: <span style={{ color: "#0f172a", fontWeight: 600 }}>{room.capacity}</span>
                </p>

                <label style={{ fontSize: "13px", fontWeight: "600", color: "#4b5563", display: "block", marginBottom: "4px" }}>
                  Оберіть дату та час початку сесії:
                </label>
                <input 
                  type="datetime-local" 
                  value={currentChosenTime}
                  onChange={(e) => handleDateChange(room.roomId, e.target.value)}
                  style={{ marginBottom: "12px" }}
                />

                {currentChosenTime && (
                  <div style={{ 
                    padding: "8px 12px", 
                    borderRadius: "6px", 
                    fontSize: "13px", 
                    fontWeight: "600", 
                    marginBottom: "16px",
                    background: isOccupied ? "#fef2f2" : "#f0fdf4",
                    color: isOccupied ? "#dc2626" : "#16a34a",
                    border: isOccupied ? "1px solid #fee2e2" : "1px solid #dcfce7",
                    textAlign: "center"
                  }}>
                    {isOccupied ? "✕ Цей проміжок часу вже ЗАЙНЯТИЙ" : "✓ Час вільний для бронювання"}
                  </div>
                )}
              </div>

              <button 
                onClick={() => handleBook(room.roomId)}
                disabled={isOccupied || !currentChosenTime}
                style={{ 
                  background: isOccupied ? "#94a3b8" : "#2563eb", 
                  color: "#fff", 
                  cursor: isOccupied ? "not-allowed" : "pointer",
                  marginTop: "auto",
                  opacity: !currentChosenTime ? 0.6 : 1
                }}
              >
                {t("user.bookBtn")}
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
}