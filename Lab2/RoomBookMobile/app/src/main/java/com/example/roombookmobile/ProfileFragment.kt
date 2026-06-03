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

class ProfileFragment : Fragment() {

    private lateinit var tvReportResult: TextView

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        val view = inflater.inflate(R.layout.fragment_profile, container, false)
        tvReportResult = view.findViewById(R.id.tvReportResult)
        val btnLoadReport = view.findViewById<Button>(R.id.btnLoadReport)

        btnLoadReport.setOnClickListener {
            val token = (activity as? MainActivity)?.jwtToken ?: return@setOnClickListener
            val tokenHeader = "Bearer $token"

            tvReportResult.text = "Генерація звіту..."

            RetrofitClient.instance.getRooms(tokenHeader).enqueue(object : Callback<List<RoomDto>> {
                override fun onResponse(call: Call<List<RoomDto>>, response: Response<List<RoomDto>>) {
                    if (response.isSuccessful && response.body() != null) {
                        val count = response.body()!!.size
                        val reportText = "ЗВІТ СИСТЕМИ\n\nВсього приміщень: $count\nСтатус системи: Активна\nРоль користувача: Administrator"
                        tvReportResult.text = reportText
                    } else {
                        tvReportResult.text = "Помилка генерації звіту."
                    }
                }
                override fun onFailure(call: Call<List<RoomDto>>, t: Throwable) {
                    tvReportResult.text = "Помилка мережі бекенду."
                }
            })
        }

        return view
    }
}