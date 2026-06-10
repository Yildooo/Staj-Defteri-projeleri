package com.example.notpaylasimi;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.util.List;

public class MainActivity extends AppCompatActivity implements NotAdapter.NotDinleyici {

    private DatabaseHelper veritabani;
    private NotAdapter adapter;
    private EditText baslikInput;
    private EditText icerikInput;
    private View notFormu;
    private TextView bosListeMesaji;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        veritabani = new DatabaseHelper(this);

        baslikInput = findViewById(R.id.inputBaslik);
        icerikInput = findViewById(R.id.inputIcerik);
        notFormu = findViewById(R.id.notFormu);
        bosListeMesaji = findViewById(R.id.textBosListe);

        RecyclerView recyclerView = findViewById(R.id.recyclerNotlar);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new NotAdapter(this);
        recyclerView.setAdapter(adapter);

        FloatingActionButton fabEkle = findViewById(R.id.fabEkle);
        fabEkle.setOnClickListener(v -> notFormunuGoster(true));

        findViewById(R.id.btnKaydet).setOnClickListener(v -> notuKaydet());
        findViewById(R.id.btnIptal).setOnClickListener(v -> notFormunuGoster(false));

        notlariYukle();
    }

    private void notFormunuGoster(boolean goster) {
        notFormu.setVisibility(goster ? View.VISIBLE : View.GONE);
        if (!goster) {
            baslikInput.setText("");
            icerikInput.setText("");
        }
    }

    private void notuKaydet() {
        String baslik = baslikInput.getText().toString().trim();
        String icerik = icerikInput.getText().toString().trim();

        if (TextUtils.isEmpty(baslik)) {
            baslikInput.setError(getString(R.string.hata_baslik_bos));
            return;
        }
        if (TextUtils.isEmpty(icerik)) {
            icerikInput.setError(getString(R.string.hata_icerik_bos));
            return;
        }

        long sonuc = veritabani.notEkle(baslik, icerik);
        if (sonuc != -1) {
            Toast.makeText(this, R.string.not_kaydedildi, Toast.LENGTH_SHORT).show();
            notFormunuGoster(false);
            notlariYukle();
        }
    }

    private void notlariYukle() {
        List<Not> notlar = veritabani.tumNotlariGetir();
        adapter.notlariGuncelle(notlar);
        bosListeMesaji.setVisibility(notlar.isEmpty() ? View.VISIBLE : View.GONE);
    }

    @Override
    public void notPaylas(Not not) {
        String notMetni = not.paylasimMetni();

        Intent intent = new Intent(Intent.ACTION_SEND);
        intent.setType("text/plain");
        intent.putExtra(Intent.EXTRA_SUBJECT, not.getBaslik());
        intent.putExtra(Intent.EXTRA_TEXT, notMetni);
        startActivity(Intent.createChooser(intent, getString(R.string.paylas_baslik)));
    }

    @Override
    public void notSil(Not not) {
        veritabani.notSil(not.getId());
        Toast.makeText(this, R.string.not_silindi, Toast.LENGTH_SHORT).show();
        notlariYukle();
    }
}
