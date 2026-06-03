package com.example.roombookmobile

import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.fragment.app.Fragment
import com.example.roombookmobile.models.LoginRequest
import com.example.roombookmobile.models.LoginResponse
import com.example.roombookmobile.network.RetrofitClient
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.navigation.NavigationBarView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class MainActivity : AppCompatActivity() {

    var jwtToken: String? = null
    var isAdmin: Boolean = false
    private var bottomNavigation: BottomNavigationView? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        showLoginScreen()
    }

    private fun showLoginScreen() {
        jwtToken = null
        setContentView(R.layout.activity_login)
        invalidateOptionsMenu()

        val etEmail = findViewById<EditText>(R.id.etLoginEmail)
        val etPassword = findViewById<EditText>(R.id.etLoginPassword)

        findViewById<Button>(R.id.btnLoginUser).setOnClickListener {
            val email = etEmail.text.toString().trim()
            val pass = etPassword.text.toString().trim()
            if (email.isEmpty() || pass.isEmpty()) {
                Toast.makeText(this, "Заповніть поля введення", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            performLogin(email, pass, false)
        }

        findViewById<Button>(R.id.btnLoginAdmin).setOnClickListener {
            val email = etEmail.text.toString().trim()
            val pass = etPassword.text.toString().trim()
            if (email.isEmpty() || pass.isEmpty()) {
                Toast.makeText(this, "Заповніть поля введення", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            performLogin(email, pass, true)
        }
    }

    private fun performLogin(email: String, pass: String, adminRole: Boolean) {
        val loginData = LoginRequest(email, pass)
        RetrofitClient.instance.login(loginData).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                if (response.isSuccessful && response.body() != null) {
                    jwtToken = response.body()!!.token
                    isAdmin = adminRole
                    initMainInterface()
                } else {
                    Toast.makeText(this@MainActivity, "Неправильний логін або пароль", Toast.LENGTH_LONG).show()
                }
            }
            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                Toast.makeText(this@MainActivity, "Збій мережі бекенду", Toast.LENGTH_LONG).show()
            }
        })
    }

    private fun initMainInterface() {
        setContentView(R.layout.activity_main)
        invalidateOptionsMenu()

        val toolbar = findViewById<Toolbar>(R.id.main_toolbar)
        setSupportActionBar(toolbar)
        supportActionBar?.title = if (isAdmin) "Панель Адміністратора" else "Кабінет Користувача"

        bottomNavigation = findViewById(R.id.bottom_navigation)
        bottomNavigation?.setOnItemSelectedListener(object : NavigationBarView.OnItemSelectedListener {
            override fun onNavigationItemSelected(item: MenuItem): Boolean {
                var selectedFragment: Fragment? = null
                when (item.itemId) {
                    R.id.nav_booking -> selectedFragment = if (isAdmin) AdminRoomsFragment() else BookingFragment()
                    R.id.nav_my_bookings -> selectedFragment = MyBookingsFragment()
                    R.id.nav_profile -> selectedFragment = ProfileFragment()
                }
                if (selectedFragment != null) {
                    supportFragmentManager.beginTransaction()
                        .replace(R.id.fragment_container, selectedFragment)
                        .commitAllowingStateLoss()
                }
                return true
            }
        })

        val startFragment = if (isAdmin) AdminRoomsFragment() else BookingFragment()
        supportFragmentManager.beginTransaction()
            .replace(R.id.fragment_container, startFragment)
            .commitAllowingStateLoss()
    }

    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        if (jwtToken != null) {
            menuInflater.inflate(R.menu.toolbar_menu, menu)
            return true
        }
        return super.onCreateOptionsMenu(menu)
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        if (item.itemId == R.id.action_logout) {
            showLoginScreen()
            return true
        }
        return super.onOptionsItemSelected(item)
    }
}