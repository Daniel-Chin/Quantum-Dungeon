class Assert {
    public Assert(bool x) {
        if (! x) throw new System.Exception(
            "Assertion failed."
        );
    }
}
