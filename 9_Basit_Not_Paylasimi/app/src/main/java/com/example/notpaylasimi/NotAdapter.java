package com.example.notpaylasimi;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;
import java.util.List;

public class NotAdapter extends RecyclerView.Adapter<NotAdapter.NotViewHolder> {

    public interface NotDinleyici {
        void notPaylas(Not not);
        void notSil(Not not);
    }

    private final List<Not> notlar = new ArrayList<>();
    private final NotDinleyici dinleyici;

    public NotAdapter(NotDinleyici dinleyici) {
        this.dinleyici = dinleyici;
    }

    public void notlariGuncelle(List<Not> yeniNotlar) {
        notlar.clear();
        notlar.addAll(yeniNotlar);
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public NotViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_not, parent, false);
        return new NotViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull NotViewHolder holder, int position) {
        Not not = notlar.get(position);
        holder.baslikText.setText(not.getBaslik());
        holder.icerikText.setText(not.getIcerik());
        holder.tarihText.setText(not.getTarih());

        holder.paylasButon.setOnClickListener(v -> dinleyici.notPaylas(not));
        holder.silButon.setOnClickListener(v -> dinleyici.notSil(not));
    }

    @Override
    public int getItemCount() {
        return notlar.size();
    }

    static class NotViewHolder extends RecyclerView.ViewHolder {
        TextView baslikText;
        TextView icerikText;
        TextView tarihText;
        ImageButton paylasButon;
        ImageButton silButon;

        NotViewHolder(@NonNull View itemView) {
            super(itemView);
            baslikText = itemView.findViewById(R.id.textBaslik);
            icerikText = itemView.findViewById(R.id.textIcerik);
            tarihText = itemView.findViewById(R.id.textTarih);
            paylasButon = itemView.findViewById(R.id.btnPaylas);
            silButon = itemView.findViewById(R.id.btnSil);
        }
    }
}
