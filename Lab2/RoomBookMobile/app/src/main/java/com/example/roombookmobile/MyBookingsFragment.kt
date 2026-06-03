package com.example.roombookmobile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.roombookmobile.models.RoomDto
import com.example.roombookmobile.network.RetrofitClient
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class MyBookingsFragment : Fragment() {

    private lateinit var tvMyBookingsList: TextView

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        val view = inflater.inflate(R.layout.fragment_my_bookings, container, false)
        tvMyBookingsList = view.findViewById(R.id.tvMyBookingsList)
        val btnRefreshMyBookings = view.findViewById<Button>(R.id.btnRefreshMyBookings)

        btnRefreshMyBookings.setOnClickListener { loadMyBookings() }
        loadMyBookings()
        return view
    }

    private fun loadMyBookings() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val tokenHeader = "Bearer $token"

        tvMyBookingsList.text = "Завантаження..."

        RetrofitClient.instance.getRooms(tokenHeader).enqueue(object : Callback<List<RoomDto>> {
            override fun onResponse(call: Call<List<RoomDto>>, response: Response<List<RoomDto>>) {
                if (response.isSuccessful && response.body() != null) {
                    val rooms = response.body()!!
                    val sb = StringBuilder("Активні приміщення для бронювання:\n\n")
                    for (room in rooms) {
                        sb.append("Назва: ").append(room.name).append("\n")
                        sb.append("Місткість: ").append(room.capacity).append(" осіб\n")
                        sb.append("------------------------\n")
                    }
                    tvMyBookingsList.text = sb.toString()
                } else {
                    tvMyBookingsList.text = "Не вдалося отримати дані."
                }
            }
            override fun onFailure(call: Call<List<RoomDto>>, t: Throwable) {
                tvMyBookingsList.text = "Помилка мережі при отриманні списку."
            }
        })
    }
}