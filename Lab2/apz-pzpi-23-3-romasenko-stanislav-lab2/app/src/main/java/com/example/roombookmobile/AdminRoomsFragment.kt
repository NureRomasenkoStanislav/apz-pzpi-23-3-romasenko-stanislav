package com.example.roombookmobile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import com.example.roombookmobile.models.RoomDto
import com.example.roombookmobile.network.RetrofitClient
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.*

class AdminRoomsFragment : Fragment() {

    private lateinit var etRoomId: EditText
    private lateinit var etRoomName: EditText
    private lateinit var etRoomCapacity: EditText
    private lateinit var tvAdminRoomsList: TextView

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        val view = inflater.inflate(R.layout.fragment_admin_rooms, container, false)

        etRoomId = view.findViewById(R.id.etRoomId)
        etRoomName = view.findViewById(R.id.etRoomName)
        etRoomCapacity = view.findViewById(R.id.etRoomCapacity)
        tvAdminRoomsList = view.findViewById(R.id.tvAdminRoomsList)

        view.findViewById<Button>(R.id.btnRefreshAdminRooms).setOnClickListener { loadRooms() }
        view.findViewById<Button>(R.id.btnAddRoom).setOnClickListener { createRoom() }
        view.findViewById<Button>(R.id.btnUpdateRoom).setOnClickListener { updateRoom() }
        view.findViewById<Button>(R.id.btnDeleteRoom).setOnClickListener { deleteRoom() }

        loadRooms()
        return view
    }

    private fun loadRooms() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val tokenHeader = "Bearer $token"

        RetrofitClient.instance.getRooms(tokenHeader).enqueue(object : Callback<List<RoomDto>> {
            override fun onResponse(call: Call<List<RoomDto>>, response: Response<List<RoomDto>>) {
                if (response.isSuccessful && response.body() != null) {
                    val sb = StringBuilder("СПИСОК КІМНАТ У БАЗІ:\n\n")
                    for (room in response.body()!!) {
                        sb.append("ID: ").append(room.roomId).append(" | ")
                        sb.append(room.name).append(" (Місць: ").append(room.capacity).append(")\n")
                    }
                    tvAdminRoomsList.text = sb.toString()
                }
            }
            override fun onFailure(call: Call<List<RoomDto>>, t: Throwable) {
                tvAdminRoomsList.text = "Помилка оновлення даних"
            }
        })
    }

    private fun createRoom() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val name = etRoomName.text.toString()
        val cap = etRoomCapacity.text.toString().toIntOrNull() ?: 0

        val roomDto = RoomDto(
            roomId = 0,
            name = name,
            capacity = cap,
            description = "Створено з мобільного пристрою",
            isArchived = false,
            isLocked = false
        )

        RetrofitClient.instance.createRoom("Bearer $token", roomDto).enqueue(object : Callback<RoomDto> {
            override fun onResponse(call: Call<RoomDto>, response: Response<RoomDto>) {
                if (response.isSuccessful) {
                    Toast.makeText(requireContext(), "Кімнату створено!", Toast.LENGTH_SHORT).show()
                    loadRooms()
                }
            }
            override fun onFailure(call: Call<RoomDto>, t: Throwable) {}
        })
    }

    private fun updateRoom() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val rid = etRoomId.text.toString().toIntOrNull() ?: return
        val name = etRoomName.text.toString()
        val cap = etRoomCapacity.text.toString().toIntOrNull() ?: 0

        val roomDto = RoomDto(
            roomId = rid,
            name = name,
            capacity = cap,
            description = "Оновлено з мобільного пристрою",
            isArchived = false,
            isLocked = false
        )

        RetrofitClient.instance.updateRoom("Bearer $token", rid, roomDto).enqueue(object : Callback<RoomDto> {
            override fun onResponse(call: Call<RoomDto>, response: Response<RoomDto>) {
                if (response.isSuccessful) {
                    Toast.makeText(requireContext(), "Дані оновлено!", Toast.LENGTH_SHORT).show()
                    loadRooms()
                }
            }
            override fun onFailure(call: Call<RoomDto>, t: Throwable) {}
        })
    }

    private fun deleteRoom() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val rid = etRoomId.text.toString().toIntOrNull() ?: return

        RetrofitClient.instance.deleteRoom("Bearer $token", rid).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    Toast.makeText(requireContext(), "Кімнату видалено!", Toast.LENGTH_SHORT).show()
                    loadRooms()
                }
            }
            override fun onFailure(call: Call<Void>, t: Throwable) {}
        })
    }
}