package com.example.roombookmobile

import android.app.DatePickerDialog
import android.app.TimePickerDialog
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.Spinner
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.roombookmobile.models.BookingDto
import com.example.roombookmobile.models.RoomDto
import com.example.roombookmobile.network.RetrofitClient
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.Calendar
import java.util.Locale

class BookingFragment : Fragment() {

    private lateinit var spinnerRooms: Spinner
    private lateinit var tvSelectedTime: TextView
    private lateinit var tvBookingStatus: TextView

    private var roomsList: List<RoomDto> = emptyList()
    private var selectedCalendar = Calendar.getInstance()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        val view = inflater.inflate(R.layout.fragment_booking, container, false)

        spinnerRooms = view.findViewById(R.id.spinnerRooms)
        tvSelectedTime = view.findViewById(R.id.tvSelectedTime)
        tvBookingStatus = view.findViewById(R.id.tvBookingStatus)

        val btnPickDateTime = view.findViewById<Button>(R.id.btnPickDateTime)
        val btnCreateBooking = view.findViewById<Button>(R.id.btnCreateBooking)
        val btnRefreshRooms = view.findViewById<Button>(R.id.btnRefreshRooms)

        btnPickDateTime.setOnClickListener {
            val currentCalendar = Calendar.getInstance()
            DatePickerDialog(
                requireContext(),
                { _, year, month, dayOfMonth ->
                    selectedCalendar.set(Calendar.YEAR, year)
                    selectedCalendar.set(Calendar.MONTH, month)
                    selectedCalendar.set(Calendar.DAY_OF_MONTH, dayOfMonth)

                    TimePickerDialog(
                        requireContext(),
                        { _, hourOfDay, minute ->
                            selectedCalendar.set(Calendar.HOUR_OF_DAY, hourOfDay)
                            selectedCalendar.set(Calendar.MINUTE, minute)
                            selectedCalendar.set(Calendar.SECOND, 0)

                            tvSelectedTime.text = String.format(
                                Locale.US, "Обрано: %04d-%02d-%02d %02d:%02d",
                                year, month + 1, dayOfMonth, hourOfDay, minute
                            )

                        },
                        currentCalendar.get(Calendar.HOUR_OF_DAY),
                        currentCalendar.get(Calendar.MINUTE),
                        true
                    ).show()
                },
                currentCalendar.get(Calendar.YEAR),
                currentCalendar.get(Calendar.MONTH),
                currentCalendar.get(Calendar.DAY_OF_MONTH)
            ).show()
        }

        btnRefreshRooms.setOnClickListener { loadRooms() }
        btnCreateBooking.setOnClickListener { sendBooking() }

        loadRooms()
        return view
    }

    private fun loadRooms() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        val tokenHeader = "Bearer $token"

        RetrofitClient.instance.getRooms(tokenHeader).enqueue(object : Callback<List<RoomDto>> {
            override fun onResponse(call: Call<List<RoomDto>>, response: Response<List<RoomDto>>) {
                if (response.isSuccessful && response.body() != null) {
                    roomsList = response.body()!!
                    val roomNames = roomsList.map { "${it.name} (Місць: ${it.capacity})" }
                    val adapter = ArrayAdapter(
                        requireContext(),
                        android.R.layout.simple_spinner_item,
                        roomNames
                    )
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                    spinnerRooms.adapter = adapter
                    tvBookingStatus.text = "Список кімнат оновлено."
                }
            }
            override fun onFailure(call: Call<List<RoomDto>>, t: Throwable) {
                tvBookingStatus.text = "Помилка завантаження кімнат."
            }
        })
    }

    private fun sendBooking() {
        val token = (activity as? MainActivity)?.jwtToken ?: return
        if (roomsList.isEmpty()) return

        val tokenHeader = "Bearer $token"
        val selectedRoomIndex = spinnerRooms.selectedItemPosition
        val chosenRoomId = selectedRoomIndex + 1

        val startYear = selectedCalendar.get(Calendar.YEAR)
        val startMonth = selectedCalendar.get(Calendar.MONTH) + 1
        val startDay = selectedCalendar.get(Calendar.DAY_OF_MONTH)
        val startHour = selectedCalendar.get(Calendar.HOUR_OF_DAY)
        val startMinute = selectedCalendar.get(Calendar.MINUTE)

        val startTimeIso = String.format(Locale.US, "%04d-%02d-%02dT%02d:%02d:00", startYear, startMonth, startDay, startHour, startMinute)
        val endTimeIso = String.format(Locale.US, "%04d-%02d-%02dT%02d:%02d:00", startYear, startMonth, startDay, startHour + 1, startMinute)

        val dynamicBooking = BookingDto(
            userId = 1,
            roomId = chosenRoomId,
            startTime = startTimeIso,
            endTime = endTimeIso,
            purpose = "Мобільне бронювання"
        )

        RetrofitClient.instance.createBooking(tokenHeader, dynamicBooking).enqueue(object :
            Callback<BookingDto> {
            override fun onResponse(call: Call<BookingDto>, response: Response<BookingDto>) {
                if (response.isSuccessful) {
                    tvBookingStatus.text = "Успішно заброньовано! ID: ${response.body()?.bookingId}"
                } else {
                    tvBookingStatus.text = "Помилка сервера: Код ${response.code()}"
                }
            }
            override fun onFailure(call: Call<BookingDto>, t: Throwable) {
                tvBookingStatus.text = "Мережева помилка."
            }
        })
    }
}