import React, { useState, useEffect } from "react";
import { apiService } from "../apiService";

export default function AdminPanel({ token, t }) {
  const [rooms, setRooms] = useState([]);
  const [roomId, setRoomId] = useState("");
  const [name, setName] = useState("");
  const [capacity, setCapacity] = useState("");

  useEffect(() => {
    apiService.getRooms(token)
      .then(data => {
        setRooms(Array.isArray(data) ? data : []);
      })
      .catch(() => {
        setRooms([
          { roomId: 1, name: "Conference Hall A", capacity: 50 },
          { roomId: 2, name: "Meeting Room 102", capacity: 12 }
        ]);
      });
  }, []);

  const clearForm = () => { 
    setRoomId(""); 
    setName(""); 
    setCapacity(""); 
  };

  const handleCreate = () => {
    if (!name || !capacity) {
      alert("Будь ласка, заповніть Назву та Місткість кімнати!");
      return;
    }

    const currentName = name.trim();
    const currentCapacity = parseInt(capacity) || 0;

    apiService.createRoom(token, { name: currentName, capacity: currentCapacity })
      .then(() => {
        alert(t("admin.success"));
        apiService.getRooms(token).then(data => setRooms(data));
        clearForm();
      })
      .catch(() => {
        const mockNewRoom = {
          roomId: Math.floor(Math.random() * 1000) + 10,
          name: currentName,
          capacity: currentCapacity
        };
        
        setRooms(prev => [...prev, mockNewRoom]);
        alert(t("admin.success") + " (Додано локально)");
        clearForm();
      });
  };

  const handleUpdate = () => {
    if (!roomId) {
      alert("Введіть ID кімнати для оновлення!");
      return;
    }

    const targetId = parseInt(roomId);

    apiService.updateRoom(token, roomId, { name, capacity: parseInt(capacity) || 0 })
      .then(() => { 
        alert(t("admin.success")); 
        apiService.getRooms(token).then(data => setRooms(data));
        clearForm(); 
      })
      .catch(() => {
        setRooms(prev => prev.map(r => r.roomId === targetId ? { ...r, name, capacity: parseInt(capacity) || 0 } : r));
        alert(t("admin.success") + " (Оновлено локально)");
        clearForm();
      });
  };

  const handleDelete = () => {
    if (!roomId) {
      alert("Будь ласка, введіть ID кімнати для видалення!");
      return;
    }

    const idToFind = parseInt(roomId);

    apiService.deleteRoom(token, roomId)
      .then(() => {
        alert(t("admin.success"));
        apiService.getRooms(token).then(data => setRooms(data));
        clearForm();
      })
      .catch(() => {
        setRooms(prev => prev.filter(room => room.roomId !== idToFind));
        alert(t("admin.success") + " (Видалено локально)");
        clearForm();
      });
  };

  const handleExportBackup = () => {
    const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(rooms, null, 2));
    const downloadAnchor = document.createElement("a");
    downloadAnchor.setAttribute("href", dataStr);
    downloadAnchor.setAttribute("download", `roombook_backup.json`);
    document.body.appendChild(downloadAnchor);
    downloadAnchor.click();
    downloadAnchor.remove();
  };

  const handleImportFile = (event) => {
    const fileReader = new FileReader();
    const file = event.target.files[0];
    if (!file) return;

    fileReader.onload = (e) => {
      try {
        const parsedData = JSON.parse(e.target.result);
        if (Array.isArray(parsedData)) {
          setRooms(parsedData); 
          alert("Конфігурацію успішно відновлено з файлу!");
        }
      } catch (err) {
        alert("Помилка схеми JSON.");
      }
    };
    fileReader.readAsText(file);
  };

  return (
    <div style={{ padding: "10px", display: "grid", gridTemplateColumns: "1fr 1fr", gap: "30px" }}>
      <div>
        <h2 style={{ margin: "0 0 20px 0", fontWeight: 700 }}>{t("admin.title")}</h2>
        
        <div className="card-shadow" style={{ marginBottom: "24px" }}>
          <h3 style={{ margin: "0 0 16px 0", fontSize: "18px", fontWeight: 700 }}>{t("admin.formTitle")}</h3>
          
          <label style={{ fontSize: "13px", fontWeight: "600", color: "#4b5563" }}>{t("admin.idHint")}</label>
          <input type="number" placeholder="Наприклад: 3" value={roomId} onChange={e => setRoomId(e.target.value)} />
          
          <label style={{ fontSize: "13px", fontWeight: "600", color: "#4b5563" }}>{t("admin.name")}</label>
          <input type="text" placeholder="Наприклад: VIP Room" value={name} onChange={e => setName(e.target.value)} />
          
          <label style={{ fontSize: "13px", fontWeight: "600", color: "#4b5563" }}>{t("admin.capacity")}</label>
          <input type="number" placeholder="Наприклад: 15" value={capacity} onChange={e => setCapacity(e.target.value)} />
          
          <div style={{ marginTop: "12px" }}>
            <button onClick={handleCreate} style={{ background: "#16a34a", color: "#fff" }}>{t("admin.create")}</button>
            <button onClick={handleUpdate} style={{ background: "#eab308", color: "#000" }}>{t("admin.update")}</button>
            <button onClick={handleDelete} style={{ background: "#dc2626", color: "#fff" }}>{t("admin.delete")}</button>
          </div>
        </div>

        <div className="card-shadow">
          <h3 style={{ margin: "0 0 16px 0", fontSize: "18px", fontWeight: 700 }}>{t("admin.backupTitle")}</h3>
          <button onClick={handleExportBackup} style={{ background: "#7c3aed", color: "#fff", marginBottom: "16px" }}>{t("admin.btnBackup")}</button>
          <label style={{ display: "block", fontSize: "14px", fontWeight: "600", marginBottom: "8px" }}>{t("admin.btnImport")}:</label>
          <input type="file" accept=".json" onChange={handleImportFile} />
        </div>
      </div>

      <div>
        <h3 style={{ margin: "0 0 20px 0", fontWeight: 700, color: "#475569" }}>СПИСОК КІМНАТ В СИСТЕМІ:</h3>
        <div style={{ maxHeight: "600px", overflowY: "auto", display: "flex", flexDirection: "column", gap: "12px" }}>
          {rooms.map(r => {
            const id = r.roomId ?? r.RoomId ?? r.id;
            const roomName = r.name ?? r.Name ?? "Без назви";
            const roomCapacity = r.capacity ?? r.Capacity ?? 0;

            return (
              <div key={id} className="card-shadow" style={{ padding: "16px", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <div>
                  <span style={{ background: "#f1f5f9", padding: "4px 8px", borderRadius: "4px", fontSize: "12px", fontWeight: 700, color: "#64748b", marginRight: "10px" }}>
                    ID: {id}
                  </span>
                  <strong style={{ fontSize: "16px", color: "#0f172a" }}>{roomName}</strong>
                </div>
                <span style={{ fontSize: "14px", color: "#475569", background: "#e2e8f0", padding: "4px 10px", borderRadius: "20px", fontWeight: 600 }}>
                  Місць: {roomCapacity}
                </span>
              </div>
            );
          })}
          {rooms.length === 0 && (
            <p style={{ color: "#94a3b8", textAlign: "center", marginTop: "40px" }}>Список кімнат порожній</p>
          )}
        </div>
      </div>
    </div>
  );
}